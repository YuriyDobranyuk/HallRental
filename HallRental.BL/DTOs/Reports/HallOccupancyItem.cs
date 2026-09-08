namespace HallRental.BL.DTOs.Reports
{
    public sealed record HallOccupancyItem(
        Guid HallId,
        string HallName,
        double BookedHours,
        int BookingsCount
    );
}
