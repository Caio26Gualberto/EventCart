namespace payment_service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMicroserviceDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHostedService<PaymentConsumer>();
            return services;
        }
    }
}
