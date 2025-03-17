using Microsoft.Extensions.DependencyInjection;
using StockManagement.Services.Interfaces;
using StockManagement.Services.Services;
using System.Reflection;

namespace StockManagement.Services
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}
