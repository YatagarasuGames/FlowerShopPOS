using FlowerShop.Application.DTO;
using FlowerShop.Domain.Entities;
using FlowerShop.Domain.Enums;
using FlowerShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Application.Services
{
    public class OrdersService
    {
        private readonly FlowersDbContext _context;
        private readonly AuditService _auditService;

        public OrdersService(FlowersDbContext context, AuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<AdvancedOrderResponseDTO> CreateOrderAsync(Guid cashierId, CreateAdvancedOrderDTO dto)
        {
            if (!dto.SingleItems.Any() && !dto.Compositions.Any())
            {
                throw new InvalidOperationException("Невозможно оформить пустой чек.");
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CashierId = cashierId,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = 0
            };

            var createdOrderItems = new List<OrderItem>();
            var responseSingleItems = new List<OrderItemReceiptDTO>();
            var responseCompositions = new List<CompositionReceiptDTO>();
            float totalOrderAmount = 0f;

            foreach (var singleItemDto in dto.SingleItems)
            {
                var allocatedItems = await AllocateBatchesFifoAsync(
                    order.Id,
                    null,
                    singleItemDto.ProductId,
                    singleItemDto.ProductBatchId,
                    singleItemDto.Quantity,
                    singleItemDto.DiscountType,
                    singleItemDto.DiscountValue);

                foreach (var item in allocatedItems)
                {
                    createdOrderItems.Add(item);
                    totalOrderAmount += item.TotalPrice;

                    var product = await _context.Products.FindAsync(item.ProductId);
                    responseSingleItems.Add(MapToReceiptItem(product?.Name ?? "Товар", item));
                }
            }

            foreach (var compDto in dto.Compositions)
            {
                if (!compDto.Items.Any())
                {
                    throw new InvalidOperationException($"Букет '{compDto.Name}' должен содержать хотя бы один элемент.");
                }

                var composition = new Composition
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    Name = compDto.Name,
                    AssemblyPrice = compDto.AssemblyPrice,
                    TotalPrice = compDto.AssemblyPrice
                };

                var compositionReceiptItems = new List<OrderItemReceiptDTO>();

                foreach (var compItemDto in compDto.Items)
                {
                    var allocatedItems = await AllocateBatchesFifoAsync(
                        order.Id,
                        composition.Id,
                        compItemDto.ProductId,
                        compItemDto.ProductBatchId,
                        compItemDto.Quantity,
                        compItemDto.DiscountType,
                        compItemDto.DiscountValue);

                    foreach (var item in allocatedItems)
                    {
                        createdOrderItems.Add(item);
                        composition.TotalPrice += item.TotalPrice;

                        var product = await _context.Products.FindAsync(item.ProductId);
                        compositionReceiptItems.Add(MapToReceiptItem(product?.Name ?? "Товар", item));
                    }
                }

                totalOrderAmount += composition.TotalPrice;
                await _context.Compositions.AddAsync(composition);

                responseCompositions.Add(new CompositionReceiptDTO
                {
                    Name = composition.Name,
                    AssemblyPrice = composition.AssemblyPrice,
                    TotalPrice = composition.TotalPrice,
                    Items = compositionReceiptItems
                });
            }

            order.TotalAmount = (float)Math.Round(totalOrderAmount, 2);

            await _context.Orders.AddAsync(order);
            await _context.OrderItems.AddRangeAsync(createdOrderItems);
            await _context.SaveChangesAsync();

            var cashier = await _context.Users.FindAsync(cashierId);
            var detailsSummary = $"Оформлен чек #{order.Id.ToString()[..8]} на сумму {order.TotalAmount:N2} ₽. " +
                                 $"Позиций: {dto.SingleItems.Count}, букетов: {dto.Compositions.Count}.";

            await _auditService.LogAsync(
                cashierId,
                cashier?.Username ?? "Кассир",
                "CreateOrder",
                "Order",
                order.Id.ToString(),
                detailsSummary);

            return new AdvancedOrderResponseDTO
            {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                SingleItems = responseSingleItems,
                Compositions = responseCompositions
            };
        }

        private async Task<List<OrderItem>> AllocateBatchesFifoAsync(
            Guid orderId,
            Guid? compositionId,
            Guid productId,
            Guid? targetBatchId,
            int requestedQuantity,
            DiscountType discountType,
            float discountValue)
        {
            if (requestedQuantity <= 0)
                throw new InvalidOperationException("Количество товара должно быть больше нуля.");

            var product = await _context.Products.FindAsync(productId);
            if (product == null || !product.IsActive)
                throw new KeyNotFoundException($"Товар с ID '{productId}' не найден.");

            var allocatedOrderItems = new List<OrderItem>();

            if (targetBatchId.HasValue && targetBatchId.Value != Guid.Empty)
            {
                var batch = await _context.ProductBatches.FindAsync(targetBatchId.Value);
                if (batch == null || batch.ProductId != productId)
                    throw new KeyNotFoundException("Указанная партия товара не найдена.");

                if (batch.RemainingQuantity < requestedQuantity)
                    throw new InvalidOperationException($"В выбранной партии доступно только {batch.RemainingQuantity} шт., запрошено {requestedQuantity} шт.");

                batch.RemainingQuantity -= requestedQuantity;
                product.Stock -= requestedQuantity;

                float finalPrice = CalculateDiscountedPrice(batch.SellingPrice, discountType, discountValue);

                allocatedOrderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    CompositionId = compositionId,
                    ProductId = productId,
                    ProductBatchId = batch.Id,
                    Quantity = requestedQuantity,
                    PurchasePrice = batch.PurchasePrice,
                    BaseUnitPrice = batch.SellingPrice,
                    DiscountType = discountType,
                    DiscountValue = discountValue,
                    FinalUnitPrice = finalPrice,
                    TotalPrice = (float)Math.Round(finalPrice * requestedQuantity, 2)
                });

                return allocatedOrderItems;
            }

            var batches = await _context.ProductBatches
                .Where(b => b.ProductId == productId && b.RemainingQuantity > 0)
                .OrderBy(b => b.ReceivedAt)
                .ToListAsync();

            int totalAvailable = batches.Sum(b => b.RemainingQuantity);
            if (totalAvailable < requestedQuantity)
                throw new InvalidOperationException($"Недостаточно товара '{product.Name}'. Доступно: {totalAvailable} шт.");

            int quantityToAllocate = requestedQuantity;

            foreach (var batch in batches)
            {
                if (quantityToAllocate == 0) break;
                int take = Math.Min(batch.RemainingQuantity, quantityToAllocate);

                batch.RemainingQuantity -= take;
                quantityToAllocate -= take;

                float finalPrice = CalculateDiscountedPrice(batch.SellingPrice, discountType, discountValue);

                allocatedOrderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    CompositionId = compositionId,
                    ProductId = productId,
                    ProductBatchId = batch.Id,
                    Quantity = take,
                    PurchasePrice = batch.PurchasePrice,
                    BaseUnitPrice = batch.SellingPrice,
                    DiscountType = discountType,
                    DiscountValue = discountValue,
                    FinalUnitPrice = finalPrice,
                    TotalPrice = (float)Math.Round(finalPrice * take, 2)
                });
            }

            product.Stock -= requestedQuantity;
            return allocatedOrderItems;
        }

        private static float CalculateDiscountedPrice(float basePrice, DiscountType type, float discountValue)
        {
            return type switch
            {
                DiscountType.Percentage =>
                    Math.Max(0, (float)Math.Round(basePrice * (1f - (Math.Clamp(discountValue, 0f, 100f) / 100f)), 2)),

                DiscountType.FixedPriceOverride =>
                    Math.Max(0, (float)Math.Round(discountValue, 2)),

                _ => basePrice
            };
        }

        private static OrderItemReceiptDTO MapToReceiptItem(string productName, OrderItem item)
        {
            string discountInfo = item.DiscountType switch
            {
                DiscountType.Percentage => $"-{item.DiscountValue}%",
                DiscountType.FixedPriceOverride => $"Уценка до {item.FinalUnitPrice} ₽",
                _ => "Без скидки"
            };

            return new OrderItemReceiptDTO
            {
                ProductName = productName,
                Quantity = item.Quantity,
                BaseUnitPrice = item.BaseUnitPrice,
                FinalUnitPrice = item.FinalUnitPrice,
                TotalPrice = item.TotalPrice,
                DiscountInfo = discountInfo
            };
        }

        public async Task<AdvancedOrderResponseDTO> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Заказ не найден.");

            var orderItems = await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();

            var compositions = await _context.Compositions
                .Where(c => c.OrderId == orderId)
                .ToListAsync();

            var responseSingleItems = orderItems
                .Where(oi => oi.CompositionId == null)
                .Select(oi => MapToReceiptItem(oi.Product?.Name ?? "Товар", oi))
                .ToList();

            var responseCompositions = new List<CompositionReceiptDTO>();
            foreach (var comp in compositions)
            {
                var compItems = orderItems
                    .Where(oi => oi.CompositionId == comp.Id)
                    .Select(oi => MapToReceiptItem(oi.Product?.Name ?? "Товар", oi))
                    .ToList();

                responseCompositions.Add(new CompositionReceiptDTO
                {
                    Name = comp.Name,
                    AssemblyPrice = comp.AssemblyPrice,
                    TotalPrice = comp.TotalPrice,
                    Items = compItems
                });
            }

            return new AdvancedOrderResponseDTO
            {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                SingleItems = responseSingleItems,
                Compositions = responseCompositions
            };
        }

        public async Task<IEnumerable<AdvancedOrderResponseDTO>> GetAllOrdersAsync(DateTime? from = null, DateTime? to = null)
        {
            var query = _context.Orders.AsQueryable();

            if (from.HasValue)
            {
                var startDate = DateTime.SpecifyKind(from.Value.Date, DateTimeKind.Utc);
                query = query.Where(o => o.CreatedAt >= startDate);
            }

            if (to.HasValue)
            {
                var endDate = DateTime.SpecifyKind(to.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
                query = query.Where(o => o.CreatedAt <= endDate);
            }

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var result = new List<AdvancedOrderResponseDTO>();
            foreach (var order in orders)
            {
                var orderDetails = await GetOrderByIdAsync(order.Id);
                result.Add(orderDetails);
            }

            return result;
        }
    }
}