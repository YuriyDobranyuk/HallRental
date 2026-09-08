namespace HallRental.BL.DTOs.Reports
{
    public sealed record RevenueReportResponse(
        DateTime FromUtc,
        DateTime ToUtc,
        int BookingsCount,
        decimal TotalRevenue,
        decimal AverageBookingValue
    );
}
