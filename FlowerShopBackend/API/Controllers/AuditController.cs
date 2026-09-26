using FlowerShop.Application.Services;
using FlowerShop.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.API.Controllers
{
    [ApiController]
    [Route("api/audit")]
    [Authorize(Roles = "Admin")]
    public class AuditController : ControllerBase
    {
        private readonly AuditService _auditService;

        public AuditController(AuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetLogs([FromQuery] int limit = 100)
        {
            var logs = await _auditService.GetLogsAsync(limit);
            return Ok(logs);
        }
    }
}