using FlowerShop.Application.DTO;
using FlowerShop.Domain.Entities;
using FlowerShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Application.Services
{
    public class WriteOffsService
    {
        private readonly FlowersDbContext _context;
        private readonly AuditService _auditService;

        public WriteOffsService(FlowersDbContext context, AuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<WriteOffResponseDTO> CreateWriteOffAsync(CreateWriteOffDTO dto)
        {
            if (dto.Quantity <= 0)
                throw new InvalidOperationException("Количество к списанию должно быть больше нуля.");

            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
                throw new KeyNotFoundException("Товар не найден.");

            if (product.Stock < dto.Quantity)
                throw new InvalidOperationException($"Недостаточно остатка на складе. Доступно: {product.Stock} шт., запрошено: {dto.Quantity} шт.");

            // Загрузка партий по FIFO
            var batches = await _context.ProductBatches
                .Where(b => b.ProductId == dto.ProductId && b.RemainingQuantity > 0)
                .OrderBy(b => b.ReceivedAt)
                .ToListAsync();

            if (!batches.Any())
            {
                var fallbackBatch = new ProductBatch
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    PurchasePrice = product.CurrentPrice * 0.6f,
                    SellingPrice = product.CurrentPrice,
                    InitialQuantity = product.Stock,
                    RemainingQuantity = product.Stock,
                    ReceivedAt = DateTime.UtcNow
                };
                await _context.ProductBatches.AddAsync(fallbackBatch);
                batches.Add(fallbackBatch);
            }

            int remainingToDeduct = dto.Quantity;
            WriteOff? primaryWriteOff = null;

            if (dto.ProductBatchId.HasValue && dto.ProductBatchId.Value != Guid.Empty)
            {
                var targetBatch = await _context.ProductBatches.FindAsync(dto.ProductBatchId.Value);
                if (targetBatch == null || targetBatch.ProductId != dto.ProductId)
                    throw new KeyNotFoundException("Указанная партия товара не найдена.");

                if (targetBatch.RemainingQuantity < dto.Quantity)
                    throw new InvalidOperationException($"В выбранной партии осталось только {targetBatch.RemainingQuantity} шт.");

                targetBatch.RemainingQuantity -= dto.Quantity;

                primaryWriteOff = new WriteOff
                {
                    Id = Guid.NewGuid(),
                    ProductId = dto.ProductId,
                    ProductBatchId = targetBatch.Id,
                    CashierId = dto.CashierId,
                    Quantity = dto.Quantity,
                    PurchasePriceAtWriteOff = targetBatch.PurchasePrice,
                    Reason = dto.Reason,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.WriteOffs.AddAsync(primaryWriteOff);
            }
            else
            {
                foreach (var batch in batches)
                {
                    if (remainingToDeduct <= 0) break;

                    int take = Math.Min(batch.RemainingQuantity, remainingToDeduct);
                    batch.RemainingQuantity -= take;
                    remainingToDeduct -= take;

                    var writeOffItem = new WriteOff
                    {
                        Id = Guid.NewGuid(),
                        ProductId = dto.ProductId,
                        ProductBatchId = batch.Id,
                        CashierId = dto.CashierId,
                        Quantity = take,
                        PurchasePriceAtWriteOff = batch.PurchasePrice,
                        Reason = dto.Reason,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _context.WriteOffs.AddAsync(writeOffItem);
                    primaryWriteOff ??= writeOffItem;
                }
            }

            product.Stock -= dto.Quantity;
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                "WriteOff",
                "Product",
                product.Id.ToString(),
                $"Списан товар '{product.Name}' в количестве {dto.Quantity} шт. Причина: '{dto.Reason}'. Себестоимость списания: {(dto.Quantity * primaryWriteOff!.PurchasePriceAtWriteOff):N2} ₽.");

            return new WriteOffResponseDTO
            {
                Id = primaryWriteOff.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = dto.Quantity,
                PurchasePriceAtWriteOff = primaryWriteOff.PurchasePriceAtWriteOff,
                RetailPriceAtWriteOff = product.CurrentPrice,
                Reason = dto.Reason,
                CreatedAt = primaryWriteOff.CreatedAt
            };
        }

        public async Task<IEnumerable<WriteOffResponseDTO>> GetAllAsync(DateTime? from = null, DateTime? to = null)
        {
            var query = _context.WriteOffs
                .Include(w => w.Product)
                .AsQueryable();

            if (from.HasValue)
            {
                var startDate = DateTime.SpecifyKind(from.Value.Date, DateTimeKind.Utc);
                query = query.Where(w => w.CreatedAt >= startDate);
            }

            if (to.HasValue)
            {
                var endDate = DateTime.SpecifyKind(to.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
                query = query.Where(o => o.CreatedAt <= endDate);
            }

            var writeOffs = await query.OrderByDescending(w => w.CreatedAt).ToListAsync();

            return writeOffs.Select(w => new WriteOffResponseDTO
            {
                Id = w.Id,
                ProductId = w.ProductId,
                ProductName = w.Product?.Name ?? "Товар",
                Quantity = w.Quantity,
                PurchasePriceAtWriteOff = w.PurchasePriceAtWriteOff,
                RetailPriceAtWriteOff = w.Product?.CurrentPrice ?? 0,
                Reason = w.Reason,
                CreatedAt = w.CreatedAt
            });
        }
    }
}