using HallRental.DAL.Data;
using HallRental.DAL.Entities;
using HallRental.DAL.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HallRental.DAL.Repositories
{
    public class BookingRepository(HallRentalDbContext context) : BaseRepository<Booking>(context), IBookingRepository
    {
        public async Task<Booking?> GetByIdWithDetailsAsync(Guid bookingId, bool asNoTracking, CancellationToken ct)
        {
            IQueryable<Booking> q = _dbSet
                .Include(b => b.Hall)
                .Include(b => b.BookingServices)
                    .ThenInclude(bs => bs.Service)
                .Where(b => b.Id == bookingId);

            if (asNoTracking) q = q.AsNoTracking();
            return await q.FirstOrDefaultAsync(ct);
        }

        public async Task<bool> HasOverlapAsync(Guid hallId, DateTime startUtc, DateTime endUtc, CancellationToken ct)
        {
            return await _dbSet.AnyAsync(b =>
            b.HallId == hallId &&
            b.StartUtc < endUtc &&
            startUtc < b.EndUtc, ct);
        }
    }
}
