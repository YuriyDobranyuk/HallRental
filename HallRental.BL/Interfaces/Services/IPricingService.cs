namespace HallRental.BL.Interfaces.Services
{
    public interface IPricingService
    {
        decimal CalculateHallPrice(DateTime startUtc, DateTime endUtc, decimal baseHourlyRate);
    }
}
