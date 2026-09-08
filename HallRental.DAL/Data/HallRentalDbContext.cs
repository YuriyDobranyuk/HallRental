using HallRental.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace HallRental.DAL.Data
{
    public class HallRentalDbContext(DbContextOptions<HallRentalDbContext> options) : DbContext(options)
    {
        public DbSet<Hall> Halls { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<HallService> HallServices { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<BookingService> BookingServices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hall>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.Name)
                    .HasMaxLength(200)
                    .IsRequired();

                e.Property(x => x.BaseHourlyRate)
                    .HasColumnType("decimal(18,2)");

                e.Property(x => x.CreatedAtUtc)
                    .HasDefaultValueSql("SYSUTCDATETIME()");

                e.HasQueryFilter(x => !x.IsDeleted);

                e.HasIndex(x => x.Capacity);
            });

            modelBuilder.Entity<Service>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.Name)
                    .HasMaxLength(200)
                    .IsRequired();

                e.HasIndex(x => x.Name).IsUnique();

                e.Property(x => x.Price)
                    .HasColumnType("decimal(18,2)");

                e.Property(x => x.CreatedAtUtc)
                    .HasDefaultValueSql("SYSUTCDATETIME()");
            });

            modelBuilder.Entity<HallService>(e =>
            {
                e.HasKey(x => new { x.HallId, x.ServiceId });

                e.HasOne(x => x.Hall)
                    .WithMany(h => h.HallServices)
                    .HasForeignKey(x => x.HallId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Service)
                    .WithMany(s => s.HallServices)
                    .HasForeignKey(x => x.ServiceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Booking>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.TotalPrice)
                    .HasColumnType("decimal(18,2)");

                e.Property(x => x.CreatedAtUtc)
                    .HasDefaultValueSql("SYSUTCDATETIME()");

                e.HasIndex(x => new { x.HallId, x.StartUtc, x.EndUtc });

                e.HasOne(x => x.Hall)
                    .WithMany(h => h.Bookings)
                    .HasForeignKey(x => x.HallId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BookingService>(e =>
            {
                e.HasKey(x => new { x.BookingId, x.ServiceId });

                e.Property(x => x.PriceAtBooking)
                    .HasColumnType("decimal(18,2)");

                e.HasOne(x => x.Booking)
                    .WithMany(b => b.BookingServices)
                    .HasForeignKey(x => x.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Service)
                    .WithMany(s => s.BookingServices)
                    .HasForeignKey(x => x.ServiceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            var now = DateTime.UtcNow;
            var hallAId = Guid.Parse("6b388f3e-0fe7-4d01-9b6b-f1c4465cb815");
            var hallBId = Guid.Parse("38e195b3-177d-4294-bfc3-dfb4fe81c723");
            var hallCId = Guid.Parse("1f9bb736-7681-41ac-9e65-d086dc30903b");
            var ProjectorId = Guid.Parse("a7f24c91-3d68-4e52-bf17-82c9d5a41e73");
            var WifiId = Guid.Parse("3e91b7d4-6a25-4f08-8c53-d129e7a46b81");
            var SoundId = Guid.Parse("c5824f16-9d37-4b60-ae28-71f3c95d824a");

            _ = modelBuilder.Entity<Hall>().HasData(
                new Hall { 
                    Id = hallAId, 
                    Name = "Зал A", 
                    Capacity = 50,
                    BaseHourlyRate = 2000m,
                    IsDeleted = false,
                    CreatedAtUtc = now
                },
                new Hall
                {
                    Id = hallBId,
                    Name = "Зал B",
                    Capacity = 100,
                    BaseHourlyRate = 3500m,
                    IsDeleted = false,
                    CreatedAtUtc = now
                },
                new Hall
                {
                    Id = hallCId,
                    Name = "Зал C",
                    Capacity = 30,
                    BaseHourlyRate = 1500m,
                    IsDeleted = false,
                    CreatedAtUtc = now
                }
            );

            _ = modelBuilder.Entity<Service>().HasData(
                new Service
                {
                    Id = ProjectorId,
                    Name = "Проєктор",
                    Price = 500m,
                    CreatedAtUtc = now
                },
                new Service
                {
                    Id = WifiId,
                    Name = "Wi-Fi",
                    Price = 300m,
                    CreatedAtUtc = now
                },
                new Service
                {
                    Id = SoundId,
                    Name = "Звук",
                    Price = 700m,
                    CreatedAtUtc = now
                }
            );

            _ = modelBuilder.Entity<HallService>().HasData(
                new HallService
                {
                    HallId = hallAId,
                    ServiceId = ProjectorId
                },
                new HallService
                {
                    HallId = hallAId,
                    ServiceId = WifiId
                },
                new HallService
                {
                    HallId = hallAId,
                    ServiceId = SoundId
                },
                new HallService
                {
                    HallId = hallBId,
                    ServiceId = ProjectorId
                },
                new HallService
                {
                    HallId = hallBId,
                    ServiceId = WifiId
                },
                new HallService
                {
                    HallId = hallBId,
                    ServiceId = SoundId
                },
                new HallService
                {
                    HallId = hallCId,
                    ServiceId = ProjectorId
                },
                new HallService
                {
                    HallId = hallCId,
                    ServiceId = WifiId
                },
                new HallService
                {
                    HallId = hallCId,
                    ServiceId = SoundId
                }
            );
        }
    }
}
