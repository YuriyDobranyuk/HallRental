using HallRental.DAL.Entities;

namespace HallRental.DAL.Interfaces.Repositories
{
    public interface IHallRepository : IBaseRepository<Hall>
    {
        Task<Hall?> GetByIdWithServicesAsync(Guid hallId, bool asNoTracking, CancellationToken ct);

        Task<Hall?> GetByIdIncludingDeletedAsync(Guid hallId, bool asNoTracking, CancellationToken ct);

        Task<ICollection<Hall>> SearchAvailableAsync(
            DateTime startUtc,
            DateTime endUtc,
            int requiredCapacity,
            CancellationToken ct);

        Task SetAllowedServicesAsync(Guid hallId, IReadOnlyCollection<Guid> serviceIds, CancellationToken ct);

        Task SoftDeleteAsync(Guid hallId, CancellationToken ct);

        Task<bool> ExistsAsync(Guid hallId, CancellationToken ct);
    }
}
