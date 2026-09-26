using FlowerShop.Application.DTO;
using FlowerShop.Domain.Entities;
using FlowerShop.Domain.Interfaces;
using FlowerShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace FlowerShop.Application.Services
{
    public class ProductsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly FlowersDbContext _context;
        private readonly AuditService _auditService;

        public ProductsService(IUnitOfWork unitOfWork, FlowersDbContext context, AuditService auditService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _auditService = auditService;
        }

        public async Task<ProductResponseDTO> CreateProductAsync(CreateProductDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new InvalidOperationException("Название товара не может быть пустым.");

            var existing = await _unitOfWork.Products.FindAsync(p => p.Name.ToLower() == dto.Name.Trim().ToLower() && p.IsActive);
            if (existing.Any())
                throw new InvalidOperationException($"Товар '{dto.Name}' уже существует.");

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                CurrentPrice = dto.Price,
                Stock = dto.CurrentQuantity,
                ImageUrl = dto.ImageUrl,
                IsActive = true
            };
            await _unitOfWork.Products.AddAsync(product);

            if (dto.CurrentQuantity > 0)
            {
                var initialBatch = new ProductBatch
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    PurchasePrice = dto.PurchasePrice,
                    SellingPrice = dto.Price,
                    InitialQuantity = dto.CurrentQuantity,
                    RemainingQuantity = dto.CurrentQuantity,
                    ReceivedAt = DateTime.UtcNow
                };
                await _context.ProductBatches.AddAsync(initialBatch);
            }

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                "CreateProduct",
                "Product",
                product.Id.ToString(),
                $"Создан товар '{product.Name}'. Начальный остаток: {dto.CurrentQuantity} шт., закупка: {dto.PurchasePrice} ₽, розница: {dto.Price} ₽.");

            return await GetProductByIdAsync(product.Id);
        }

        public async Task<IEnumerable<ProductResponseDTO>> GetAllProductsAsync(bool onlyActive = false)
        {
            var query = _context.Products.AsQueryable();
            if (onlyActive) query = query.Where(p => p.IsActive);

            var products = await query.OrderBy(p => p.Name).ToListAsync();
            var activeBatches = await _context.ProductBatches
                .Where(b => b.RemainingQuantity > 0)
                .OrderBy(b => b.ReceivedAt)
                .ToListAsync();

            return products.Select(product =>
            {
                var batches = activeBatches
                    .Where(b => b.ProductId == product.Id)
                    .Select(b => new BatchPriceInfoDTO
                    {
                        BatchId = b.Id,
                        PurchasePrice = b.PurchasePrice,
                        SellingPrice = b.SellingPrice,
                        RemainingQuantity = b.RemainingQuantity,
                        ReceivedAt = b.ReceivedAt
                    })
                    .ToList();

                return new ProductResponseDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.CurrentPrice,
                    MinPrice = batches.Any() ? batches.Min(b => b.SellingPrice) : product.CurrentPrice,
                    MaxPrice = batches.Any() ? batches.Max(b => b.SellingPrice) : product.CurrentPrice,
                    Stock = product.Stock,
                    IsActive = product.IsActive,
                    ImageUrl = product.ImageUrl,
                    ActiveBatches = batches
                };
            });
        }

        public async Task<ProductResponseDTO> GetProductByIdAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) throw new KeyNotFoundException("Товар не найден.");

            var batches = await _context.ProductBatches
                .Where(b => b.ProductId == id && b.RemainingQuantity > 0)
                .OrderBy(b => b.ReceivedAt)
                .Select(b => new BatchPriceInfoDTO
                {
                    BatchId = b.Id,
                    PurchasePrice = b.PurchasePrice,
                    SellingPrice = b.SellingPrice,
                    RemainingQuantity = b.RemainingQuantity,
                    ReceivedAt = b.ReceivedAt
                })
                .ToListAsync();

            return new ProductResponseDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.CurrentPrice,
                MinPrice = batches.Any() ? batches.Min(b => b.SellingPrice) : product.CurrentPrice,
                MaxPrice = batches.Any() ? batches.Max(b => b.SellingPrice) : product.CurrentPrice,
                Stock = product.Stock,
                IsActive = product.IsActive,
                ImageUrl = product.ImageUrl,
                ActiveBatches = batches
            };
        }

        public async Task UpdateBatchPriceAsync(Guid batchId, float newSellingPrice)
        {
            if (newSellingPrice < 0)
                throw new InvalidOperationException("Цена не может быть отрицательной.");

            var batch = await _context.ProductBatches
                .Include(b => b.Product)
                .FirstOrDefaultAsync(b => b.Id == batchId);

            if (batch == null)
                throw new KeyNotFoundException("Партия товара не найдена.");

            var oldPrice = batch.SellingPrice;
            batch.SellingPrice = newSellingPrice;

            var latestBatch = await _context.ProductBatches
                .Where(b => b.ProductId == batch.ProductId && b.RemainingQuantity > 0)
                .OrderByDescending(b => b.ReceivedAt)
                .FirstOrDefaultAsync();

            if (latestBatch != null)
            {
                var product = await _context.Products.FindAsync(batch.ProductId);
                if (product != null)
                {
                    product.CurrentPrice = latestBatch.SellingPrice;
                }
            }

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                "UpdateBatchPrice",
                "ProductBatch",
                batch.Id.ToString(),
                $"Изменена розничная цена партии товара '{batch.Product?.Name ?? "Товар"}' с {oldPrice} ₽ на {newSellingPrice} ₽. Остаток в партии: {batch.RemainingQuantity} шт.");
        }

        public async Task UpdatePriceAsync(Guid productId, float newPrice)
        {
            if (newPrice < 0)
                throw new InvalidOperationException("Цена товара не может быть отрицательной.");

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
                throw new KeyNotFoundException("Товар не найден.");

            var oldPrice = product.CurrentPrice;
            product.CurrentPrice = newPrice;
            _unitOfWork.Products.Update(product);

            var history = new PriceHistory
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                OldPrice = oldPrice,
                NewPrice = newPrice,
                ChangedAt = DateTime.UtcNow
            };
            await _context.PriceHistory.AddAsync(history);

            await _unitOfWork.SaveChangesAsync();

            await _auditService.LogAsync(
                "UpdateProductPrice",
                "Product",
                product.Id.ToString(),
                $"Изменена базовая розничная цена товара '{product.Name}' с {oldPrice} ₽ на {newPrice} ₽.");
        }

        public async Task SoftDeleteProductAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Товар не найден.");

            product.IsActive = false;
            _unitOfWork.Products.Update(product);

            await _unitOfWork.SaveChangesAsync();

            await _auditService.LogAsync(
                "DeleteProduct",
                "Product",
                product.Id.ToString(),
                $"Товар '{product.Name}' снят с продажи (деактивирован).");
        }

        public async Task UpdateProductNameAsync(Guid productId, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new InvalidOperationException("Название товара не может быть пустым.");

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
                throw new KeyNotFoundException("Товар не найден.");

            var trimmed = newName.Trim();
            var exists = await _context.Products.AnyAsync(p => p.Id != productId && p.Name.ToLower() == trimmed.ToLower() && p.IsActive);
            if (exists)
                throw new InvalidOperationException($"Товар с названием '{trimmed}' уже существует в каталоге.");

            var oldName = product.Name;
            product.Name = trimmed;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            await _auditService.LogAsync(
                "UpdateProductName",
                "Product",
                product.Id.ToString(),
                $"Название товара изменено с '{oldName}' на '{trimmed}'.");
        }

        public async Task ToggleProductStatusAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Товар не найден.");

            if (product.IsActive)
            {
                if (product.Stock > 0)
                {
                    throw new InvalidOperationException($"Нельзя снять товар с продажи, пока на складе есть остаток ({product.Stock} шт.). Сначала оформите списание или продайте остаток.");
                }
                product.IsActive = false;
            }
            else
            {
                product.IsActive = true;
            }

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            await _auditService.LogAsync(
                product.IsActive ? "RestoreProduct" : "DeactivateProduct",
                "Product",
                product.Id.ToString(),
                product.IsActive
                    ? $"Товар '{product.Name}' возвращен в продажу."
                    : $"Товар '{product.Name}' снят с продажи.");
        }

        public async Task<int> OptimizeExistingImagesAsync()
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                return 0;

            var products = await _context.Products
                .Where(p => !string.IsNullOrEmpty(p.ImageUrl))
                .ToListAsync();

            int optimizedCount = 0;

            foreach (var product in products)
            {
                var relativePath = product.ImageUrl!.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

                if (!File.Exists(fullPath))
                    continue;

                try
                {
                    var fileInfo = new FileInfo(fullPath);
                    // Если файл уже WebP и весит меньше 70 КБ — пропускаем
                    if (fileInfo.Extension.Equals(".webp", StringComparison.OrdinalIgnoreCase) && fileInfo.Length < 70 * 1024)
                    {
                        continue;
                    }

                    var newFileName = $"{Guid.NewGuid()}.webp";
                    var newFullPath = Path.Combine(uploadsFolder, newFileName);

                    using (var image = await Image.LoadAsync(fullPath))
                    {
                        if (image.Width > 600 || image.Height > 600)
                        {
                            image.Mutate(x => x.Resize(new ResizeOptions
                            {
                                Size = new Size(600, 600),
                                Mode = ResizeMode.Max
                            }));
                        }

                        var encoder = new WebpEncoder { Quality = 75 };
                        await image.SaveAsync(newFullPath, encoder);
                    }

                    // Удаляем старый файл
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }

                    // Обновляем запись в БД
                    product.ImageUrl = $"/uploads/{newFileName}";
                    optimizedCount++;
                }
                catch
                {
                    // Пропускаем поврежденные файлы без падения цикла
                    continue;
                }
            }

            if (optimizedCount > 0)
            {
                await _context.SaveChangesAsync();
                await _auditService.LogAsync(
                    "OptimizeImages",
                    "Products",
                    null,
                    $"Оптимизировано существующих изображений: {optimizedCount} шт.");
            }

            return optimizedCount;
        }
    }


}