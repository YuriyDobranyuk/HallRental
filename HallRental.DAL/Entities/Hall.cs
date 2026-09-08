namespace HallRental.DAL.Entities
{
    public class Hall
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public int Capacity { get; set; }

        public decimal BaseHourlyRate { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public ICollection<HallService> HallServices { get; set; } = new List<HallService>();

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
