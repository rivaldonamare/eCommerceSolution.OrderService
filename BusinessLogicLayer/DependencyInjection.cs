namespace BusinessLogicLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
    {
        // Add services to the container
        services.AddAutoMapper(typeof(OrderMappingProfile).Assembly);
        services.AddValidatorsFromAssemblyContaining<CreateOrderRequestValidator>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = $"{configuration["REDIS_HOST"]}:{configuration["REDIS_PORT"]}";
        });

        return services;
    }
}
