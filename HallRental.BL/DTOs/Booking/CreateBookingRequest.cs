namespace HallRental.BL.DTOs.Booking
{
    public sealed record CreateBookingRequest(
        Guid HallId,
        DateTime StartUtc,
        int DurationMinutes,
        IReadOnlyList<Guid>? SelectedServiceIds
    );
}
