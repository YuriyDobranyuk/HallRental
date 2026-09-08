using HallRental.BL.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HallRental.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportsController(IReportsService reports, ILogger<ReportsController> logger) : ControllerBase
    {
        [HttpGet("revenue")]
        public async Task<IActionResult> Revenue([FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc, CancellationToken ct)
        {
            logger.LogInformation("Reports.Revenue started. FromUtc={FromUtc}, ToUtc={ToUtc}", fromUtc, toUtc);
            
            var result = await reports.GetRevenueAsync(fromUtc, toUtc, ct);
            
            logger.LogInformation("Reports.Revenue succeeded. FromUtc={FromUtc}, ToUtc={ToUtc}", fromUtc, toUtc);
            
            return Ok(result);
        }

        [HttpGet("hall-occupancy")]
        public async Task<IActionResult> HallOccupancy([FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc, CancellationToken ct)
        {
            logger.LogInformation("Reports.HallOccupancy started. FromUtc={FromUtc}, ToUtc={ToUtc}", fromUtc, toUtc);
            
            var result = await reports.GetHallOccupancyAsync(fromUtc, toUtc, ct);
            
            logger.LogInformation("Reports.HallOccupancy succeeded. Count={Count}", result.Count);
            
            return Ok(result);
        }

        [HttpGet("top-services")]
        public async Task<IActionResult> TopServices([FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc, CancellationToken ct)
        {
            logger.LogInformation("Reports.TopServices started. FromUtc={FromUtc}, ToUtc={ToUtc}", fromUtc, toUtc);
            
            var result = await reports.GetTopServicesAsync(fromUtc, toUtc, ct);
            
            logger.LogInformation("Reports.TopServices succeeded. Count={Count}", result.Count);
            
            return Ok(result);
        }
    }
}
