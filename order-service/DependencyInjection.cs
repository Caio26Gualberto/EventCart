using Microsoft.EntityFrameworkCore;
using order_service.Context;

namespace order_service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMicroserviceDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = "Server=localhost,1433;Database=OrderDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True";

            services.AddDbContext<OrderDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddSingleton<KafkaProducer>();
            services.AddHostedService<OrderConsumer>();
            return services;
        }
    }
}
