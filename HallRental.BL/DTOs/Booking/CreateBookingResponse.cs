namespace HallRental.BL.DTOs.Booking
{
    public sealed record CreateBookingResponse(
        Guid BookingId,
        decimal TotalPrice
    );
}
