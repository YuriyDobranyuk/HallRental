using HallRental.BL.Interfaces.Services;

namespace HallRental.BL.Services
{
    public class PricingService : IPricingService
    {
        public decimal CalculateHallPrice(DateTime startUtc, DateTime endUtc, decimal baseHourlyRate)
        {
            if (endUtc <= startUtc) throw new ArgumentException("EndUtc must be > StartUtc.");

            if (baseHourlyRate <= 0) throw new ArgumentException("BaseHourlyRate must be > 0.");

            ValidateNoForbiddenTime(startUtc, endUtc);

            decimal total = 0m;

            var day = startUtc.Date;
            var lastDay = endUtc.Date;

            while (day <= lastDay)
            {
                var allowedFrom = day.AddHours(6);
                var allowedTo = day.AddHours(23);

                var sliceStart = Max(startUtc, allowedFrom);
                var sliceEnd = Min(endUtc, allowedTo);

                if (sliceEnd > sliceStart)
                    total += CalculateDaySlice(sliceStart, sliceEnd, baseHourlyRate);

                day = day.AddDays(1);
            }

            return Math.Round(total, 2, MidpointRounding.AwayFromZero);
        }

        private static decimal CalculateDaySlice(DateTime startUtc, DateTime endUtc, decimal baseHourlyRate)
        {
            var day = startUtc.Date;

            var t06 = day.AddHours(6);
            var t09 = day.AddHours(9);
            var t12 = day.AddHours(12);
            var t14 = day.AddHours(14);
            var t18 = day.AddHours(18);
            var t23 = day.AddHours(23);

            var segments = new[]
            {
                (From: t06, To: t09, Mult: 0.90m),
                (From: t09, To: t12, Mult: 1.00m),
                (From: t12, To: t14, Mult: 1.15m),
                (From: t14, To: t18, Mult: 1.00m),
                (From: t18, To: t23, Mult: 0.80m)
            };

            decimal subtotal = 0m;
            foreach (var s in segments)
            {
                var from = Max(startUtc, s.From);
                var to = Min(endUtc, s.To);
                if (to <= from) continue;

                var hours = (decimal)(to - from).TotalMinutes / 60m;
                subtotal += hours * baseHourlyRate * s.Mult;
            }

            return subtotal;
        }

        private static void ValidateNoForbiddenTime(DateTime startUtc, DateTime endUtc)
        {
            var day = startUtc.Date;
            var lastDay = endUtc.Date;

            while (day <= lastDay)
            {
                var dayStart = day;
                var dayEnd = day.AddDays(1);

                var sliceStart = Max(startUtc, dayStart);
                var sliceEnd = Min(endUtc, dayEnd);

                if (sliceEnd > sliceStart)
                {
                    var forbiddenMorningStart = day;
                    var forbiddenMorningEnd = day.AddHours(6);

                    var forbiddenNightStart = day.AddHours(23);
                    var forbiddenNightEnd = day.AddDays(1);

                    if (Overlaps(sliceStart, sliceEnd, forbiddenMorningStart, forbiddenMorningEnd) ||
                        Overlaps(sliceStart, sliceEnd, forbiddenNightStart, forbiddenNightEnd))
                        throw new InvalidOperationException("Booking must be within 06:00–23:00 UTC without night gaps.");
                }

                day = day.AddDays(1);
            }
        }

        private static bool Overlaps(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd)
            => aStart < bEnd && bStart < aEnd;

        private static DateTime Max(DateTime a, DateTime b) => a > b ? a : b;

        private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;
    }
}
