using HallRental.DAL.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HallRental.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServicesController(IUnitOfWork unitOfWork, ILogger<ServicesController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            logger.LogInformation("Services.GetAll started.");

            var services = await unitOfWork.Services.GetAllAsync(ct);
            var dto = services.Select(s => new { s.Id, s.Name, s.Price }).ToList();

            logger.LogInformation("Services.GetAll succeeded. Count={Count}", dto.Count);
            return Ok(dto);
        }
    }
}
