using HallRental.DAL.Entities;

namespace HallRental.DAL.Interfaces.Repositories
{
    public interface IServiceRepository : IBaseRepository<Service>
    {
        Task<IReadOnlyList<Service>> GetAllAsync(CancellationToken ct);

        Task<IReadOnlyList<Service>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct);

        Task<bool> AllExistAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct);
    }
}
