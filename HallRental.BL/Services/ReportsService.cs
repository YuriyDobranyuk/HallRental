using HallRental.BL.DTOs.Reports;
using HallRental.BL.Interfaces.Services;
using HallRental.DAL.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace HallRental.BL.Services
{
    public class ReportsService(IUnitOfWork unitOfWork,
        ILogger<BookingManager> logger) : IReportsService
    {
        public async Task<RevenueReportResponse> GetRevenueAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct)
        {
            if (toUtc <= fromUtc) throw new ArgumentException("toUtc must be > fromUtc.");

            var bookingsQuery = unitOfWork.Bookings.Query()
                .Where(b => b.StartUtc < toUtc && fromUtc < b.EndUtc);

            var bookingsCount = await bookingsQuery.CountAsync(ct);
            var totalRevenue = await bookingsQuery.SumAsync(b => (decimal?)b.TotalPrice, ct) ?? 0m;
            var avg = bookingsCount == 0 ? 0m : totalRevenue / bookingsCount;

            var result = new RevenueReportResponse(
                FromUtc: fromUtc,
                ToUtc: toUtc,
                BookingsCount: bookingsCount,
                TotalRevenue: Math.Round(totalRevenue, 2),
                AverageBookingValue: Math.Round(avg, 2)
            );

            logger.LogInformation("Reports.Revenue completed. FromUtc={FromUtc}, ToUtc={ToUtc}, " +
                "BookingsCount={BookingsCount}, TotalRevenue={TotalRevenue}, " +
                "AverageBookingValue={AverageBookingValue}.",
                fromUtc, toUtc, result.BookingsCount, result.TotalRevenue, result.AverageBookingValue);

            return result;
        }

        public async Task<IReadOnlyList<HallOccupancyItem>> GetHallOccupancyAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct)
        {
            if (toUtc <= fromUtc) throw new ArgumentException("toUtc must be > fromUtc.");

            var data = await unitOfWork.Bookings.Query()
                .Where(b => b.StartUtc < toUtc && fromUtc < b.EndUtc)
                .Select(b => new
                {
                    b.HallId,
                    HallName = b.Hall.Name,
                    Start = b.StartUtc,
                    End = b.EndUtc
                })
                .ToListAsync(ct);

            var grouped = data
                .GroupBy(x => new { x.HallId, x.HallName })
                .Select(g => new HallOccupancyItem(
                    HallId: g.Key.HallId,
                    HallName: g.Key.HallName,
                    BookedHours: g.Sum(x => (ClampEnd(x.End, toUtc) - ClampStart(x.Start, fromUtc)).TotalMinutes) / 60.0,
                    BookingsCount: g.Count()
                ))
                .OrderByDescending(x => x.BookedHours)
                .ToList();

            logger.LogInformation("Reports.HallOccupancy completed. FromUtc={FromUtc}, ToUtc={ToUtc}, " +
                "ItemsCount={ItemsCount}.", fromUtc, toUtc, grouped.Count);

            return grouped;

            static DateTime ClampStart(DateTime value, DateTime min) => value < min ? min : value;

            static DateTime ClampEnd(DateTime value, DateTime max) => value > max ? max : value;
        }

        public async Task<IReadOnlyList<ServicePopularityItem>> GetTopServicesAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct)
        {
            if (toUtc <= fromUtc) throw new ArgumentException("toUtc must be > fromUtc.");

            var rows = await unitOfWork.BookingServices.Query()
                .Where(bs => bs.Booking.StartUtc < toUtc && fromUtc < bs.Booking.EndUtc)
                .GroupBy(bs => new { bs.ServiceId, ServiceName = bs.Service.Name })
                .Select(g => new
                {
                    g.Key.ServiceId,
                    g.Key.ServiceName,
                    TimesSelected = g.Count(),
                    Revenue = g.Sum(x => x.PriceAtBooking)
                })
                .OrderByDescending(x => x.TimesSelected)
                .ToListAsync(ct);

            logger.LogInformation("Reports.TopServices completed. FromUtc={FromUtc}, ToUtc={ToUtc}, " +
                "ItemsCount={ItemsCount}.", fromUtc, toUtc, rows.Count);

            return rows
                .Select(x => new ServicePopularityItem(x.ServiceId, x.ServiceName, x.TimesSelected, x.Revenue))
                .ToList();
        }
    }
}
