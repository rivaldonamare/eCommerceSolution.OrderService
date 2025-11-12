namespace DataAccessLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        // Add services to the container
        var connectionStringTemplate = configuration.GetConnectionString("MongoDb");
        var connectionString = connectionStringTemplate?.Replace("$MONGO_HOST", Environment.GetEnvironmentVariable("MONGO_HOST") ?? "localhost")
            .Replace("$MONGO_PORT", Environment.GetEnvironmentVariable("MONGO_PORT") ?? "27017");

        services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));

        services.AddScoped<IMongoDatabase>(sp =>
        {
            var mongoClient = sp.GetRequiredService<IMongoClient>();
            return mongoClient.GetDatabase("OrdersDatabase");
        });


        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}
