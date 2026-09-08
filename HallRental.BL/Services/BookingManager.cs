using HallRental.BL.DTOs.Booking;
using HallRental.BL.Interfaces.Services;
using HallRental.BL.Mappers;
using HallRental.DAL.Entities;
using HallRental.DAL.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace HallRental.BL.Services
{
    public class BookingManager(
        IUnitOfWork unitOfWork,
        IPricingService pricing,
        ILogger<BookingManager> logger) : IBookingManager
    {
        public async Task<CreateBookingResponse> CreateAsync(CreateBookingRequest request, CancellationToken ct)
        {
            var endUtc = request.StartUtc.AddMinutes(request.DurationMinutes);

            logger.LogInformation(
            "Booking.Create started. HallId={HallId}, StartUtc={StartUtc}, EndUtc={EndUtc}, SelectedServicesCount={SelectedServicesCount}",
            request.HallId, request.StartUtc, endUtc, request.SelectedServiceIds?.Count ?? 0);

            if (request.DurationMinutes <= 0)
            {
                logger.LogWarning("Booking.Create rejected: invalid time range. DurationMinutes must be > 0." +
                    "HallId={HallId}", request.HallId);

                throw new ArgumentException("DurationMinutes must be > 0.");
            }
            
            // hall with allowed services
            var hall = await unitOfWork.Halls.GetByIdWithServicesAsync(request.HallId, asNoTracking: true, ct);
            if (hall is null)
            {
                logger.LogWarning("Booking.Create rejected: hall not found. HallId={HallId}", request.HallId);

                throw new KeyNotFoundException("Hall not found.");
            }

            // overlap check
            if (await unitOfWork.Bookings.HasOverlapAsync(request.HallId, request.StartUtc, endUtc, ct))
            {
                logger.LogWarning(
                    "Booking.Create rejected: hall is not available (overlap). HallId={HallId}, StartUtc={StartUtc}, EndUtc={EndUtc}",
                    request.HallId, request.StartUtc, endUtc);

                throw new InvalidOperationException("Hall is not available for this time.");
            }
            
            var selectedIds = (request.SelectedServiceIds ?? Array.Empty<Guid>())
                    .Where(id => id != Guid.Empty)
                    .Distinct()
                    .ToArray();

            List<Service> services = new();
            decimal servicesPrice = 0m;

            if (selectedIds.Length > 0)
            {
                if (!await unitOfWork.Services.AllExistAsync(selectedIds, ct))
                {
                    logger.LogWarning(
                        "Booking.Create rejected: one or more services do not exist. " +
                        "HallId={HallId}, ServiceIds={ServiceIds}",
                        request.HallId, selectedIds);

                    throw new InvalidOperationException("One or more selected services do not exist.");
                }
                
                var allowedServiceIds = hall.HallServices.Select(x => x.ServiceId).ToHashSet();

                if (selectedIds.Any(id => !allowedServiceIds.Contains(id)))
                {
                    logger.LogWarning("One or more selected services are not available for this hall.");

                    throw new InvalidOperationException("One or more selected services are not available for this hall.");
                }

                services = (await unitOfWork.Services.GetByIdsAsync(selectedIds, ct)).ToList();
            }

            var hallPrice = pricing.CalculateHallPrice(request.StartUtc, endUtc, hall.BaseHourlyRate);

            servicesPrice = services.Sum(s => s.Price);

            var total = hallPrice + servicesPrice;

            var booking = request.ToEntity(total, DateTime.UtcNow);

            booking.EndUtc = endUtc;

            foreach (var s in services)
            {
                booking.BookingServices.Add(new BookingServiceDto(booking.Id, s.Id, s.Price).ToEntity());
            }

            await unitOfWork.Bookings.AddAsync(booking, ct);

            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation(
                "Booking.Create succeeded. BookingId={BookingId}, HallId={HallId}, " +
                "StartUtc={StartUtc}, EndUtc={EndUtc}, HallPrice={HallPrice}, " +
                "ServicesPrice={ServicesPrice}, TotalPrice={TotalPrice}, " +
                "SelectedServicesCount={SelectedServicesCount}",
                booking.Id, booking.HallId, booking.StartUtc, booking.EndUtc, 
                hallPrice, servicesPrice, booking.TotalPrice, selectedIds.Length);

            return booking.ToDto();
        }
    }
}
