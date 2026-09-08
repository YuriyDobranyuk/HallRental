using HallRental.BL.DTOs.Booking;
using HallRental.BL.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HallRental.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingsController(
        IBookingManager bookingManager, 
        ILogger<BookingsController> logger) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(CreateBookingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CreateBookingResponse>> Create([FromBody] CreateBookingRequest request, CancellationToken ct)
        {
            logger.LogInformation(
                "Bookings.Create request received. HallId={HallId}, " +
                "StartUtc={StartUtc}, DurationMinutes={DurationMinutes}, " +
                "SelectedServicesCount={SelectedServicesCount}",
                request.HallId, request.StartUtc, request.DurationMinutes, request.SelectedServiceIds?.Count ?? 0);

            var response = await bookingManager.CreateAsync(request, ct);

            logger.LogInformation(
                "Bookings.Create succeeded. BookingId={BookingId}, HallId={HallId}, TotalPrice={TotalPrice}",
                response.BookingId, request.HallId, response.TotalPrice);

            return Ok(response);
        }
    }
}
