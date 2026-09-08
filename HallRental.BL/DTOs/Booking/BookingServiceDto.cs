namespace HallRental.BL.DTOs.Booking
{
    public sealed record BookingServiceDto(
        Guid BookingId,
        Guid ServiceId,
        decimal PriceAtBooking
    );
}
