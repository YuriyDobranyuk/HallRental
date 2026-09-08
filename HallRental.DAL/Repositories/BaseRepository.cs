using HallRental.DAL.Data;
using HallRental.DAL.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HallRental.DAL.Repositories
{
    public class BaseRepository<T>(HallRentalDbContext context) : IBaseRepository<T> where T : class
    {
        protected readonly HallRentalDbContext _context = context;
        protected readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task AddAsync(T entity, CancellationToken ct)
        {
            await _dbSet.AddAsync(entity, ct);
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }

        public IQueryable<T> Query(bool asNoTracking = true)
        {
            return asNoTracking ? _dbSet.AsNoTracking() : _dbSet.AsQueryable();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
