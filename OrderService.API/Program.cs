var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer();

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

var app = builder.Build();

// Cors
app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
