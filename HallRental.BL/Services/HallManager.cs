using HallRental.BL.DTOs.Hall;
using HallRental.BL.Interfaces.Services;
using HallRental.BL.Mappers;
using HallRental.DAL.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace HallRental.BL.Services
{
    public class HallManager(IUnitOfWork unitOfWork, ILogger<HallManager> logger) : IHallManager
    {
        public async Task<Guid> CreateAsync(HallRequest request, CancellationToken ct)
        {
            var serviceIds = (request.AvailableServiceIds ?? Array.Empty<Guid>()).Distinct().ToArray();
            if (!await unitOfWork.Services.AllExistAsync(serviceIds, ct))
                throw new InvalidOperationException("One or more services do not exist.");

            logger.LogInformation("Starting hall creation process for hall name: '{HallName}'.", request.Name);

            var hall = request.ToEntity();
            hall.CreatedAtUtc = DateTime.UtcNow;
            hall.IsDeleted = false;

            await unitOfWork.Halls.AddAsync(hall, ct);
            await unitOfWork.SaveChangesAsync(ct);

            await unitOfWork.Halls.SetAllowedServicesAsync(hall.Id, serviceIds, ct);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Successfully created hall with ID '{HallId}'.", hall.Id);

            return hall.Id;
        }

        public async Task DeleteAsync(Guid hallId, CancellationToken ct)
        {
            var hall = await unitOfWork.Halls.GetByIdWithServicesAsync(hallId, true, ct);
            if (hall is null) throw new KeyNotFoundException("Hall not found.");

            await unitOfWork.Halls.SoftDeleteAsync(hall.Id, ct);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Successfully soft delete hall with ID '{HallId}'.", hallId);
        }

        public async Task<ICollection<HallDto>> GetAvailableAsync(AvailableHallsQuery query, CancellationToken ct)
        {
            logger.LogInformation("Starting all halls fetching.");

            var halls = await unitOfWork.Halls.SearchAvailableAsync(query.StartUtc, query.EndUtc, query.Capacity, ct);

            logger.LogInformation("Retrieved {HallsCount} halls.", halls.Count);

            return halls.ToDtoList();
        }

        public async Task<HallDto> GetByIdAsync(Guid hallId, CancellationToken ct)
        {
            logger.LogInformation("Getting hall by id: '{HallId}'.", hallId);

            var hall = await unitOfWork.Halls.GetByIdWithServicesAsync(hallId, true, ct);
            if (hall is null) throw new KeyNotFoundException("Hall not found.");

            return hall.ToDto();
        }

        public async Task UpdateAsync(Guid hallId, HallRequest request, CancellationToken ct)
        {
            var hall = await unitOfWork.Halls.GetByIdIncludingDeletedAsync(hallId, false, ct);
            if (hall is null) throw new KeyNotFoundException("Hall not found.");

            var serviceIds = (request.AvailableServiceIds ?? Array.Empty<Guid>()).Distinct().ToArray();
            if (!await unitOfWork.Services.AllExistAsync(serviceIds, ct))
                throw new InvalidOperationException("One or more services do not exist.");

            request.ToEntityForUpdate(hall);

            if (hall.IsDeleted) 
            {
                hall.IsDeleted = false;
                logger.LogInformation("Hall activated during update. HallId={HallId}, WasDeleted={WasDeleted}",
                    hallId, hall.IsDeleted);
            };
           
            await unitOfWork.Halls.SetAllowedServicesAsync(hallId, serviceIds, ct);

            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Successfully update hall with ID '{HallId}'.", hall.Id);
        }
    }
}
