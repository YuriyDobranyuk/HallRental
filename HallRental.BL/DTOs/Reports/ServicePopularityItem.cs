namespace HallRental.BL.DTOs.Reports
{
    public sealed record ServicePopularityItem(
        Guid ServiceId,
        string ServiceName,
        int TimesSelected,
        decimal RevenueFromService
    );
}
