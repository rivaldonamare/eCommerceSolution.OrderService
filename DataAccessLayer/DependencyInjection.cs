namespace DataAccessLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        // Build connection string template from appsettings (supports placeholders)
        var connectionStringTemplate = configuration.GetConnectionString("MongoDb");

        // Support both MONGO_* and MONGODB_* environment variable names
        var mongoHost = Environment.GetEnvironmentVariable("MONGO_HOST")
                        ?? Environment.GetEnvironmentVariable("MONGODB_HOST")
                        ?? "localhost";
        var mongoPort = Environment.GetEnvironmentVariable("MONGO_PORT")
                        ?? Environment.GetEnvironmentVariable("MONGODB_PORT")
                        ?? "27017";

        var connectionString = connectionStringTemplate?
            .Replace("$MONGO_HOST", mongoHost)
            .Replace("$MONGO_PORT", mongoPort)
            .Replace("$MONGODB_HOST", mongoHost)
            .Replace("$MONGODB_PORT", mongoPort)
            ?? $"mongodb://{mongoHost}:{mongoPort}";

        // Create MongoClient
        var mongoClient = new MongoClient(connectionString);

        // Simple retry/ping to wait for MongoDB to be ready (helps with docker-compose startup order)
        var maxRetries = 10;
        var delay = TimeSpan.FromSeconds(1);
        var connected = false;
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                mongoClient.ListDatabaseNames();
                connected = true;
                break;
            }
            catch
            {
                Thread.Sleep(delay);
            }
        }

        if (!connected)
        {
            // Let application continue and throw connection error later, but at least attempted retries
            // Alternatively, you may choose to throw here to stop the app startup
        }

        services.AddSingleton<IMongoClient>(sp => mongoClient);

        services.AddScoped<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var databaseName = configuration.GetSection("ConnectionStrings")["MongoDbDatabase"] ?? "OrdersDatabase";
            return client.GetDatabase(databaseName);
        });

        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}
