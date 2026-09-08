using HallRental.DAL.Data;
using HallRental.DAL.Entities;
using HallRental.DAL.Interfaces.Repositories;

namespace HallRental.DAL.Repositories
{
    public class UnitOfWork(
        HallRentalDbContext context,
        IBaseRepository<BookingService> bookingsServices,
        IHallRepository halls,
        IServiceRepository services,
        IBookingRepository bookings) : IUnitOfWork
    {
        public IBaseRepository<BookingService> BookingServices { get; } = bookingsServices;

        public IHallRepository Halls { get; } = halls;

        public IServiceRepository Services { get; } = services;

        public IBookingRepository Bookings { get; } = bookings;

        public Task<int> SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
    }
}
