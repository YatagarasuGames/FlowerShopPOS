using FlowerShop.Application.DTO;
using FlowerShop.Domain.Entities;
using FlowerShop.Domain.Interfaces;
using FlowerShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Application.Services
{
    public class SuppliesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly FlowersDbContext _context;
        private readonly AuditService _auditService;

        public SuppliesService(IUnitOfWork unitOfWork, FlowersDbContext context, AuditService auditService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _auditService = auditService;
        }

        public async Task<SupplyResponseDTO> CreateSupplyAsync(CreateSupplyDTO dto)
        {
            if (dto.Quantity <= 0)
                throw new InvalidOperationException("Количество поставляемого товара должно быть больше нуля.");

            if (dto.PurchasePrice < 0 || dto.SellingPrice < 0)
                throw new InvalidOperationException("Цены не могут быть отрицательными.");

            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new KeyNotFoundException("Товар не найден.");

            var user = await _unitOfWork.Users.GetByIdAsync(dto.ReceivedByUserId);
            if (user == null)
                throw new KeyNotFoundException("Пользователь не найден.");

            var supply = new Supply
            {
                Id = Guid.NewGuid(),
                ProductId = dto.ProductId,
                ReceivedByUserId = dto.ReceivedByUserId,
                Quantity = dto.Quantity,
                PurchasePrice = dto.PurchasePrice,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Supplies.AddAsync(supply);

            var batch = new ProductBatch
            {
                Id = Guid.NewGuid(),
                ProductId = dto.ProductId,
                SupplyId = supply.Id,
                PurchasePrice = dto.PurchasePrice,
                SellingPrice = dto.SellingPrice,
                InitialQuantity = dto.Quantity,
                RemainingQuantity = dto.Quantity,
                ReceivedAt = supply.CreatedAt
            };
            await _context.ProductBatches.AddAsync(batch);

            product.Stock += dto.Quantity;
            product.CurrentPrice = dto.SellingPrice;
            _unitOfWork.Products.Update(product);

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                "CreateSupply",
                "Supply",
                supply.Id.ToString(),
                $"Принята поставка товара '{product.Name}': {dto.Quantity} шт. (Закупка: {dto.PurchasePrice} ₽/шт, розница: {dto.SellingPrice} ₽/шт, общая сумма закупки: {(dto.Quantity * dto.PurchasePrice):N2} ₽).");

            return new SupplyResponseDTO
            {
                Id = supply.Id,
                ProductId = supply.ProductId,
                ProductName = product.Name,
                ReceivedByUserId = supply.ReceivedByUserId,
                Quantity = supply.Quantity,
                PurchasePrice = supply.PurchasePrice,
                SellingPrice = batch.SellingPrice,
                CreatedAt = supply.CreatedAt
            };
        }

        public async Task<IEnumerable<SupplyResponseDTO>> GetAllAsync(DateTime? from = null, DateTime? to = null)
        {
            var query = _context.Supplies
                .Include(s => s.Product)
                .AsQueryable();

            if (from.HasValue)
            {
                var startDate = DateTime.SpecifyKind(from.Value.Date, DateTimeKind.Utc);
                query = query.Where(s => s.CreatedAt >= startDate);
            }

            if (to.HasValue)
            {
                var endDate = DateTime.SpecifyKind(to.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
                query = query.Where(s => s.CreatedAt <= endDate);
            }

            var supplies = await query
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return supplies.Select(s => new SupplyResponseDTO
            {
                Id = s.Id,
                ProductId = s.ProductId,
                ProductName = s.Product?.Name ?? "Неизвестно",
                ReceivedByUserId = s.ReceivedByUserId,
                Quantity = s.Quantity,
                PurchasePrice = s.PurchasePrice,
                SellingPrice = s.PurchasePrice,
                CreatedAt = s.CreatedAt
            });
        }
    }
}