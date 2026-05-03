using catalog_service.Context;
using Microsoft.EntityFrameworkCore;

namespace catalog_service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMicroserviceDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = "Server=localhost,1433;Database=CatalogDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True";

            services.AddDbContext<CatalogDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddSingleton<KafkaProducer>();
            return services;
        }
    }
}
