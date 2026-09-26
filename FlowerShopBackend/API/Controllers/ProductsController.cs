using FlowerShop.API.Extensions;
using FlowerShop.Application.DTO;
using FlowerShop.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;

namespace FlowerShop.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsService _productsService;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSizeInBytes = 5 * 1024 * 1024;

        public ProductsController(ProductsService productsService)
        {
            _productsService = productsService;
        }

        [Authorize(Roles = "Cashier,Owner,Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> GetAll([FromQuery] bool onlyActive = false)
        {
            var products = await _productsService.GetAllProductsAsync(onlyActive);
            return Ok(products);
        }

        [Authorize(Roles = "Cashier,Owner,Admin")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductResponseDTO>> GetById(Guid id)
        {
            try
            {
                var product = await _productsService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Owner,Admin")]
        [HttpPost]
        public async Task<ActionResult<ProductResponseDTO>> Create([FromBody] CreateProductDTO dto)
        {
            try
            {
                var createdProduct = await _productsService.CreateProductAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Owner,Admin")]
        [HttpPatch("{id:guid}/price")]
        public async Task<IActionResult> UpdatePrice(Guid id, [FromBody] UpdatePriceDTO dto)
        {
            try
            {
                await _productsService.UpdatePriceAsync(id, dto.NewPrice);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Owner,Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _productsService.SoftDeleteProductAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("upload-image")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Файл не выбран." });
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
            {
                return BadRequest(new { message = "Недопустимый формат файла. Разрешены только JPG, PNG и WEBP." });
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Новые файлы сразу сохраняются в сжатом формате WebP
            var uniqueFileName = $"{Guid.NewGuid()}.webp";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var inputStream = file.OpenReadStream())
            using (var image = await Image.LoadAsync(inputStream))
            {
                // Пропорционально уменьшаем до 600px по большей стороне
                if (image.Width > 600 || image.Height > 600)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(600, 600),
                        Mode = ResizeMode.Max
                    }));
                }

                // Сжатие в WebP с качеством 75% (вес файла ~25-45 КБ вместо 3-8 МБ)
                var encoder = new WebpEncoder { Quality = 75 };
                await image.SaveAsync(filePath, encoder);
            }

            var relativeUrl = $"/uploads/{uniqueFileName}";
            return Ok(new { url = relativeUrl });
        }

        [Authorize(Roles = "Owner,Admin")]
        [HttpPatch("batches/{batchId:guid}/price")]
        public async Task<IActionResult> UpdateBatchPrice(Guid batchId, [FromBody] UpdateBatchPriceDTO dto)
        {
            try
            {
                await _productsService.UpdateBatchPriceAsync(batchId, dto.NewSellingPrice);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Owner,Admin")]
        [HttpPatch("{id:guid}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                await _productsService.ToggleProductStatusAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Owner,Admin")]
        [HttpPatch("{id:guid}/name")]
        public async Task<IActionResult> UpdateName(Guid id, [FromBody] UpdateProductNameDTO dto)
        {
            try
            {
                await _productsService.UpdateProductNameAsync(id, dto.Name);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("optimize-existing-images")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OptimizeExistingImages()
        {
            try
            {
                var count = await _productsService.OptimizeExistingImagesAsync();
                return Ok(new { message = $"Оптимизация успешно завершена. Обработано изображений: {count} шт." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка при оптимизации: {ex.Message}" });
            }
        }
    }
}