using System.ComponentModel.DataAnnotations;

namespace HallRental.DAL.Entities
{
    public class Service
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public decimal Price { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public ICollection<HallService> HallServices { get; set; } = new List<HallService>();

        public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
    }
}
