var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer(builder.Configuration);

// Add Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add fluent validation
builder.Services.AddFluentValidationAutoValidation();

// Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Register HttpClients for external services
var usersHost = builder.Configuration["UsersMicroserviceName"];
var usersPort = builder.Configuration["UsersMicroservicePort"];
var productsHost = builder.Configuration["ProductMicroserviceName"];
var productsPort = builder.Configuration["ProductMicroservicePort"];

builder.Services.AddHttpClient<UserServiceClient>(c =>
{
    c.BaseAddress = new Uri($"http://{usersHost}:{usersPort}");
});

builder.Services.AddHttpClient<ProductServiceClient>(c =>
{
    c.BaseAddress = new Uri($"http://{productsHost}:{productsPort}");
});

var app = builder.Build();

// Cors
app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
