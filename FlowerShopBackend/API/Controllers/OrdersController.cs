using FlowerShop.API.Extensions;
using FlowerShop.Application.DTO;
using FlowerShop.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize(Roles = "Cashier,Owner,Admin")]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersService _ordersService;

        public OrdersController(OrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        [HttpPost]
        public async Task<ActionResult<AdvancedOrderResponseDTO>> CreateOrder([FromBody] CreateAdvancedOrderDTO dto)
        {
            try
            {
                var cashierId = User.GetUserId();
                var result = await _ordersService.CreateOrderAsync(cashierId, dto);
                return CreatedAtAction(nameof(GetOrderById), new { id = result.OrderId }, result);
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

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AdvancedOrderResponseDTO>> GetOrderById(Guid id)
        {
            try
            {
                var order = await _ordersService.GetOrderByIdAsync(id);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdvancedOrderResponseDTO>>> GetAllOrders(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var orders = await _ordersService.GetAllOrdersAsync(from, to);
            return Ok(orders);
        }
    }
}