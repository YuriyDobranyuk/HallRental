using HallRental.BL.DTOs.Booking;

namespace HallRental.BL.Interfaces.Services
{
    public interface IBookingManager
    {
        Task<CreateBookingResponse> CreateAsync(CreateBookingRequest request, CancellationToken ct);
    }
}
