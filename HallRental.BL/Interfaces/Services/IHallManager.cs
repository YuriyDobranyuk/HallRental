using HallRental.BL.DTOs.Hall;

namespace HallRental.BL.Interfaces.Services
{
    public interface IHallManager
    {
        Task<Guid> CreateAsync(HallRequest request, CancellationToken ct);

        Task UpdateAsync(Guid hallId, HallRequest request, CancellationToken ct);

        Task DeleteAsync(Guid hallId, CancellationToken ct);

        Task<HallDto> GetByIdAsync(Guid hallId, CancellationToken ct);

        Task<ICollection<HallDto>> GetAvailableAsync(AvailableHallsQuery query, CancellationToken ct);
    }
}
