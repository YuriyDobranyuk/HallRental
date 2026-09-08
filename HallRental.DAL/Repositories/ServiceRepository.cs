using HallRental.DAL.Data;
using HallRental.DAL.Entities;
using HallRental.DAL.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HallRental.DAL.Repositories
{
    public class ServiceRepository(HallRentalDbContext context) : BaseRepository<Service>(context), IServiceRepository
    {
        public async Task<bool> AllExistAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct)
        {
            var distinct = ids.Distinct().ToArray();
            if (distinct.Length == 0) return true;

            var count = await _dbSet.CountAsync(s => distinct.Contains(s.Id), ct);
            return count == distinct.Length;
        }

        public async Task<IReadOnlyList<Service>> GetAllAsync(CancellationToken ct)
        {
            return await _dbSet.AsNoTracking().OrderBy(s => s.Name).ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Service>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct)
        {
            var distinct = ids.Distinct().ToArray();
            if (distinct.Length == 0) return Array.Empty<Service>();

            return await _dbSet.AsNoTracking()
                .Where(s => distinct.Contains(s.Id))
                .ToListAsync(ct);
        }
    }
}
