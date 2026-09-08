namespace HallRental.DAL.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }

        public Guid HallId { get; set; }

        public Hall Hall { get; set; } = default!;

        public DateTime StartUtc { get; set; }

        public DateTime EndUtc { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
    }
}
