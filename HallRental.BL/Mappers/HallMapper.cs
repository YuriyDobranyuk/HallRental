using HallRental.BL.DTOs.Hall;
using HallRental.BL.DTOs.Service;
using HallRental.DAL.Entities;
using Riok.Mapperly.Abstractions;

namespace HallRental.BL.Mappers
{
    [Mapper]
    public static partial class HallMapper
    {
        [MapperIgnoreSource(nameof(Hall.IsDeleted))]
        [MapperIgnoreSource(nameof(Hall.CreatedAtUtc))]
        [MapperIgnoreSource(nameof(Hall.Bookings))]
        [MapProperty(nameof(Hall.HallServices), nameof(HallDto.Services))]
        public static partial HallDto ToDto(this Hall hall);

        public static partial ICollection<HallDto> ToDtoList(this ICollection<Hall> halls);

        [MapperIgnoreTarget(nameof(Hall.Id))]
        [MapperIgnoreTarget(nameof(Hall.IsDeleted))]
        [MapperIgnoreTarget(nameof(Hall.CreatedAtUtc))]
        [MapperIgnoreTarget(nameof(Hall.HallServices))]
        [MapperIgnoreTarget(nameof(Hall.Bookings))]
        [MapperIgnoreSource(nameof(HallRequest.AvailableServiceIds))]
        public static partial Hall ToEntity(this HallRequest dto);

        [MapperIgnoreTarget(nameof(Hall.Id))]
        [MapperIgnoreTarget(nameof(Hall.IsDeleted))]
        [MapperIgnoreTarget(nameof(Hall.CreatedAtUtc))]
        [MapperIgnoreTarget(nameof(Hall.HallServices))]
        [MapperIgnoreTarget(nameof(Hall.Bookings))]
        [MapperIgnoreSource(nameof(HallRequest.AvailableServiceIds))]
        public static partial void ToEntityForUpdate(this HallRequest dto, Hall entity);

        private static ServiceDto MapHallService(HallService hs)
        => new(hs.Service.Id, hs.Service.Name, hs.Service.Price);
    }
}
