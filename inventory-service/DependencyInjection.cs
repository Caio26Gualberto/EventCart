using inventory_service.Context;
using Microsoft.EntityFrameworkCore;

namespace inventory_service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMicroserviceDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = "Server=localhost,1433;Database=InventoryDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True";

            services.AddDbContext<InventoryDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddHostedService<InventoryConsumer>();
            return services;
        }
    }
}
