namespace HallRental.DAL.Entities
{
    public class BookingService
    {
        public Guid BookingId { get; set; }

        public Booking Booking { get; set; } = default!;

        public Guid ServiceId { get; set; }

        public Service Service { get; set; } = default!;

        public decimal PriceAtBooking { get; set; }
    }
}
