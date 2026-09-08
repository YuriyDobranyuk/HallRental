using HallRental.DAL.Data;
using HallRental.DAL.Interfaces.Repositories;
using HallRental.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HallRental.DAL
{
    public static class DataLayerRegistration
    {
        public static IServiceCollection AddDataAccessLayerInjection(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddDbContext<HallRentalDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IHallRepository, HallRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
