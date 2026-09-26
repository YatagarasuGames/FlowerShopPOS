using FlowerShop.API.Extensions;
using FlowerShop.Application.DTO;
using FlowerShop.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.API.Controllers
{
    [ApiController]
    [Route("api/writeoffs")]
    [Authorize(Roles = "Cashier,Owner,Admin")]
    public class WriteOffsController : ControllerBase
    {
        private readonly WriteOffsService _writeOffsService;

        public WriteOffsController(WriteOffsService writeOffsService)
        {
            _writeOffsService = writeOffsService;
        }

        [HttpPost]
        public async Task<ActionResult<WriteOffResponseDTO>> CreateWriteOff([FromBody] CreateWriteOffDTO dto)
        {
            try
            {
                dto.CashierId = User.GetUserId();
                var result = await _writeOffsService.CreateWriteOffAsync(dto);
                return Ok(result);
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WriteOffResponseDTO>>> GetAllWriteOffs(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var writeOffs = await _writeOffsService.GetAllAsync(from, to);
            return Ok(writeOffs);
        }
    }
}