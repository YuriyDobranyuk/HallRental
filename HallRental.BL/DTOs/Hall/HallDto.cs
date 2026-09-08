using HallRental.BL.DTOs.Service;

namespace HallRental.BL.DTOs.Hall
{
    public sealed record HallDto(
        Guid Id,
        string Name,
        int Capacity,
        decimal BaseHourlyRate,
        IReadOnlyList<ServiceDto> Services
    );
}
