namespace HallRental.DAL.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct);

        Task AddAsync(T entity, CancellationToken ct);

        void Update(T entity);

        void Remove(T entity);

        IQueryable<T> Query(bool asNoTracking = true);

        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
