using HallRental.BL.DTOs.Hall;
using HallRental.BL.Interfaces.Services;
using HallRental.DAL.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HallRental.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HallsController(IHallManager hallManager, ILogger<HallsController> logger) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] HallRequest request, CancellationToken ct)
        {
            logger.LogInformation(
                "Halls.Create started. Name={Name}, Capacity={Capacity}," +
                " BaseHourlyRate={BaseHourlyRate}, ServicesCount={ServicesCount}",
                request.Name, request.Capacity, request.BaseHourlyRate, request.AvailableServiceIds?.Count ?? 0);

            var id = await hallManager.CreateAsync(request, ct);

            logger.LogInformation("Halls.Create succeeded. HallId={HallId}", id);
            return Ok(id);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] HallRequest request, CancellationToken ct)
        {
            logger.LogInformation(
                "Halls.Update started. HallId={HallId}, Name={Name}, " +
                "Capacity={Capacity}, BaseHourlyRate={BaseHourlyRate}, ServicesCount={ServicesCount}",
                id, request.Name, request.Capacity, request.BaseHourlyRate, request.AvailableServiceIds?.Count ?? 0);

            await hallManager.UpdateAsync(id, request, ct);

            logger.LogInformation("Halls.Update succeeded. HallId={HallId}", id);

            return Ok();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            logger.LogInformation("Halls.Delete started. HallId={HallId}", id);

            await hallManager.DeleteAsync(id, ct);

            logger.LogInformation("Halls.Delete succeeded. HallId={HallId}", id);

            return Ok();
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(HallDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HallDto>> GetById(Guid id, CancellationToken ct)
        {
            logger.LogInformation("Halls.GetById started. HallId={HallId}", id);

            var hall = await hallManager.GetByIdAsync(id, ct);

            logger.LogInformation("Halls.GetById succeeded. HallId={HallId}", id);

            return Ok(hall);
        }

        [HttpGet("available")]
        [ProducesResponseType(typeof(ICollection<HallDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<HallDto>>> GetAvailable(
            [FromQuery] DateTime startUtc,
            [FromQuery] DateTime endUtc,
            [FromQuery] int capacity,
            CancellationToken ct)
        {
            logger.LogInformation(
                "Halls.GetAvailable started. StartUtc={StartUtc}, EndUtc={EndUtc}, Capacity={Capacity}",
                startUtc, endUtc, capacity);

            var result = await hallManager.GetAvailableAsync(new AvailableHallsQuery(startUtc, endUtc, capacity), ct);

            logger.LogInformation(
                "Halls.GetAvailable succeeded. StartUtc={StartUtc}, " +
                "EndUtc={EndUtc}, Capacity={Capacity}, ResultCount={ResultCount}",
                startUtc, endUtc, capacity, result.Count);

            return Ok(result);
        }
    }
}
