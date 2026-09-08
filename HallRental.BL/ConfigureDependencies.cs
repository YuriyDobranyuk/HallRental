using HallRental.BL.Interfaces.Services;
using HallRental.BL.Services;
using HallRental.DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HallRental.BL
{
    public static class ConfigureDependencies
    {
        public static IServiceCollection AddApplicationInjection(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddBussinessLayerInjection();
            services.AddDataAccessLayerInjection(configuration);
            return services;
        }

        public static IServiceCollection AddBussinessLayerInjection(this IServiceCollection services)
        {
            services.AddScoped<IPricingService, PricingService>();
            services.AddScoped<IHallManager, HallManager>();
            services.AddScoped<IBookingManager, BookingManager>();
            services.AddScoped<IReportsService, ReportsService>();

            return services;
        }
    }
}
