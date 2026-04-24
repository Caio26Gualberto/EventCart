using Microsoft.EntityFrameworkCore;
using payment_service.Context;

namespace payment_service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMicroserviceDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = "Server=localhost,1433;Database=PaymentDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True";

            services.AddDbContext<PaymentDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddSingleton<KafkaProducer>();
            services.AddHostedService<PaymentConsumer>();
            return services;
        }
    }
}
