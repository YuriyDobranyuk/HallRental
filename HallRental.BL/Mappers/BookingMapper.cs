using HallRental.BL.DTOs.Booking;
using HallRental.DAL.Entities;
using Riok.Mapperly.Abstractions;

namespace HallRental.BL.Mappers
{
    [Mapper]
    public static partial class BookingMapper
    {
        [MapperIgnoreSource(nameof(Booking.HallId))]
        [MapperIgnoreSource(nameof(Booking.Hall))]
        [MapperIgnoreSource(nameof(Booking.StartUtc))]
        [MapperIgnoreSource(nameof(Booking.EndUtc))]
        [MapperIgnoreSource(nameof(Booking.CreatedAtUtc))]
        [MapperIgnoreSource(nameof(Booking.BookingServices))]
        [MapProperty(nameof(Booking.Id), nameof(CreateBookingResponse.BookingId))]
        public static partial CreateBookingResponse ToDto(this Booking booking);

        [MapperIgnoreTarget(nameof(Booking.Id))]
        [MapperIgnoreTarget(nameof(Booking.EndUtc))]
        [MapperIgnoreTarget(nameof(Booking.Hall))]
        [MapperIgnoreTarget(nameof(Booking.BookingServices))]
        [MapperIgnoreSource(nameof(CreateBookingRequest.DurationMinutes))]
        [MapperIgnoreSource(nameof(CreateBookingRequest.SelectedServiceIds))]
        public static partial Booking ToEntity(this CreateBookingRequest dto,
            decimal totalPrice,
            DateTime createdAtUtc);

        [MapperIgnoreTarget(nameof(BookingService.Booking))]
        [MapperIgnoreTarget(nameof(BookingService.Service))]
        public static partial BookingService ToEntity(this BookingServiceDto dto);

    }
}
