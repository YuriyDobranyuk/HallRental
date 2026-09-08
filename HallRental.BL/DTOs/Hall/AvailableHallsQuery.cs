namespace HallRental.BL.DTOs.Hall
{
    public sealed record AvailableHallsQuery(
        DateTime StartUtc,
        DateTime EndUtc,
        int Capacity
    );
}
