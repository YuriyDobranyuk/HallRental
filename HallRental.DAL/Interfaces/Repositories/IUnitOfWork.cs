using HallRental.DAL.Entities;

namespace HallRental.DAL.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        IBaseRepository<BookingService> BookingServices { get; }

        IHallRepository Halls { get; }

        IServiceRepository Services { get; }

        IBookingRepository Bookings { get; }

        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
