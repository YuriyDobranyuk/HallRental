namespace HallRental.BL.DTOs.Booking
{
    public sealed record PriceBreakdown(
        decimal HallPrice,
        decimal ServicesPrice,
        decimal TotalPrice
    );
}
