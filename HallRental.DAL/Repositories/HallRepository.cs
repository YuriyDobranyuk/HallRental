using HallRental.DAL.Data;
using HallRental.DAL.Entities;
using HallRental.DAL.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HallRental.DAL.Repositories
{
    public class HallRepository(HallRentalDbContext context) : BaseRepository<Hall>(context), IHallRepository
    {
        public async Task<bool> ExistsAsync(Guid hallId, CancellationToken ct)
        {
            return await _dbSet.AnyAsync(h => h.Id == hallId, ct);
        }

        public async Task<Hall?> GetByIdWithServicesAsync(Guid hallId, bool asNoTracking, CancellationToken ct)
        {
            IQueryable<Hall> q = _dbSet
                .Include(h => h.HallServices)
                .ThenInclude(hs => hs.Service)
                .Where(h => h.Id == hallId);

            if (asNoTracking) q = q.AsNoTracking();
            return await q.FirstOrDefaultAsync(ct);
        }

        public async Task<Hall?> GetByIdIncludingDeletedAsync(Guid hallId, bool asNoTracking, CancellationToken ct)
        {
            IQueryable<Hall> q = _dbSet
                .IgnoreQueryFilters()
                .Where(h => h.Id == hallId);

            if (asNoTracking) q = q.AsNoTracking();

            return await q.FirstOrDefaultAsync(ct);
        }

        public async Task<ICollection<Hall>> SearchAvailableAsync(DateTime startUtc, DateTime endUtc, int requiredCapacity, CancellationToken ct)
        {
            return await _dbSet
            .AsNoTracking()
            .Where(h => h.Capacity >= requiredCapacity)
            .Where(h => !_context.Bookings.Any(b =>
                b.HallId == h.Id &&
                b.StartUtc < endUtc &&
                startUtc < b.EndUtc))
            .Include(h => h.HallServices)
            .ThenInclude(hs => hs.Service)
            .ToListAsync(ct);
        }

        public async Task SetAllowedServicesAsync(Guid hallId, IReadOnlyCollection<Guid> serviceIds, CancellationToken ct)
        {
            var desired = serviceIds.Distinct().ToHashSet();

            var current = await _context.HallServices
                .Where(x => x.HallId == hallId)
                .ToListAsync(ct);

            var currentIds = current.Select(x => x.ServiceId).ToHashSet();

            var toRemove = current.Where(x => !desired.Contains(x.ServiceId)).ToList();
            if (toRemove.Count > 0)
                _context.HallServices.RemoveRange(toRemove);

            var toAdd = desired
                .Where(id => !currentIds.Contains(id))
                .Select(id => new HallService { HallId = hallId, ServiceId = id })
                .ToList();

            if (toAdd.Count > 0)
                await _context.HallServices.AddRangeAsync(toAdd, ct);
        }

        public async Task SoftDeleteAsync(Guid hallId, CancellationToken ct)
        {
            var hall = await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(h => h.Id == hallId, ct);
            if (hall is null) return;

            hall.IsDeleted = true;
        }
    }
}
