using System.Security.Claims;
using FlowerShop.API.Extensions;
using FlowerShop.Application.DTO;
using FlowerShop.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _usersService;

        public UsersController(UsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserAdminDTO>>> GetAll()
        {
            var users = await _usersService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<ActionResult<UserAdminDTO>> Create([FromBody] CreateStaffUserDTO dto)
        {
            try
            {
                var result = await _usersService.CreateUserAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPatch("{id:guid}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentAdminId = Guid.TryParse(adminIdClaim, out var parsedId) ? parsedId : Guid.Empty;

                await _usersService.ToggleUserStatusAsync(id, currentAdminId);
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("{id:guid}/reset-password")]
        public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordDTO dto)
        {
            try
            {
                await _usersService.ResetPasswordAsync(id, dto.NewPassword);
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}