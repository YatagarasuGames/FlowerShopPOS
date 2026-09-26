using FlowerShop.API.Extensions;
using FlowerShop.Application.DTO;
using FlowerShop.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.API.Controllers
{
    [ApiController]
    [Route("api/supplies")]
    [Authorize(Roles = "Cashier,Owner,Admin")]
    public class SuppliesController : ControllerBase
    {
        private readonly SuppliesService _suppliesService;

        public SuppliesController(SuppliesService suppliesService)
        {
            _suppliesService = suppliesService;
        }

        [HttpPost]
        public async Task<ActionResult<SupplyResponseDTO>> CreateSupply([FromBody] CreateSupplyDTO dto)
        {
            try
            {
                dto.ReceivedByUserId = User.GetUserId();
                var result = await _suppliesService.CreateSupplyAsync(dto);
                return CreatedAtAction(nameof(GetAllSupplies), new { id = result.Id }, result);
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
        public async Task<ActionResult<IEnumerable<SupplyResponseDTO>>> GetAllSupplies(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var supplies = await _suppliesService.GetAllAsync(from, to);
            return Ok(supplies);
        }
    }
}