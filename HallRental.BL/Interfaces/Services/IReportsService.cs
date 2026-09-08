using HallRental.BL.DTOs.Reports;

namespace HallRental.BL.Interfaces.Services
{
    public interface IReportsService
    {
        Task<RevenueReportResponse> GetRevenueAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct);

        Task<IReadOnlyList<HallOccupancyItem>> GetHallOccupancyAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct);
        
        Task<IReadOnlyList<ServicePopularityItem>> GetTopServicesAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct);
    }
}
