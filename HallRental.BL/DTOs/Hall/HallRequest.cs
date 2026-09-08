namespace HallRental.BL.DTOs.Hall
{
    public sealed record HallRequest(
        string Name,
        int Capacity,
        decimal BaseHourlyRate,
        IReadOnlyList<Guid> AvailableServiceIds
    );
}
