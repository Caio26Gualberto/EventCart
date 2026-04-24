namespace order_service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMicroserviceDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<KafkaProducer>();
            return services;
        }
    }
}
