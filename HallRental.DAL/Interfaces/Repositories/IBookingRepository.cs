using HallRental.DAL.Entities;

namespace HallRental.DAL.Interfaces.Repositories
{
    public interface IBookingRepository : IBaseRepository<Booking>
    {
        Task<bool> HasOverlapAsync(Guid hallId, DateTime startUtc, DateTime endUtc, CancellationToken ct);

        Task<Booking?> GetByIdWithDetailsAsync(Guid bookingId, bool asNoTracking, CancellationToken ct);
    }
}
