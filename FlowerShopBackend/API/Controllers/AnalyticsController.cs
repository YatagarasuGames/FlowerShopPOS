using FlowerShop.Application.DTO;
using FlowerShop.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.API.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    [Authorize(Roles = "Owner,Admin")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AnalyticsService _analyticsService;

        public AnalyticsController(AnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("financial-summary")]
        public async Task<ActionResult<FinancialSummaryDTO>> GetFinancialSummary(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var summary = await _analyticsService.GetFinancialSummaryAsync(from, to);
            return Ok(summary);
        }

        [HttpGet("top-products")]
        public async Task<ActionResult<List<TopProductDTO>>> GetTopProducts(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int limit = 5)
        {
            var topProducts = await _analyticsService.GetTopSellingProductsAsync(from, to, limit);
            return Ok(topProducts);
        }

        [HttpGet("report")]
        public async Task<ActionResult<AnalyticsReportDTO>> GetFullReport(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int topLimit = 5)
        {
            var report = await _analyticsService.GetFullReportAsync(from, to, topLimit);
            return Ok(report);
        }
    }
}