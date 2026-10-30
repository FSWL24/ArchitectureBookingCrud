using ArchitectureBookingCrud.Api.Core.Adapters.Persistence;
using ArchitectureBookingCrud.Api.Core.Domain.Interfaces;
using ArchitectureBookingCrud.Api.Core.Domain.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var persistenceProvider = builder.Configuration["Persistence:Provider"] ?? "InMemory";
if (string.Equals(persistenceProvider, "SQLite", StringComparison.OrdinalIgnoreCase))
{
    var connectionString = builder.Configuration.GetConnectionString("Bookings")
        ?? "Data Source=bookings.db";
    builder.Services.AddDbContext<BookingSqliteDbContext>(options => options.UseSqlite(connectionString));
}
else if (!string.Equals(persistenceProvider, "InMemory", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException(
        $"Unsupported persistence provider '{persistenceProvider}'. Use 'InMemory' or 'SQLite'.");
}

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Architectural Booking CRUD API", Version = "v1" });
});

builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<BookingRepository>();
builder.Services.AddScoped<SqliteBookingRepository>();
builder.Services.AddScoped<BookingRepositoryFactory>();
builder.Services.AddScoped<IBookingRepository>(serviceProvider =>
    serviceProvider.GetRequiredService<BookingRepositoryFactory>().Create());
builder.Services.AddSingleton<InMemoryDbContext>();

var app = builder.Build();

if (string.Equals(persistenceProvider, "SQLite", StringComparison.OrdinalIgnoreCase))
{
    await using var scope = app.Services.CreateAsyncScope();
    var database = scope.ServiceProvider.GetRequiredService<BookingSqliteDbContext>();
    await database.Database.EnsureCreatedAsync();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nombre de tu API V1");
        // URL de la interfaz de usuario de Swagger (opcional)
        c.RoutePrefix = "api"; // Configura la ra�z de la URL donde se servir� Swagger UI, por ejemplo "/" para la ra�z del sitio
    });
}

// Configure the HTTP request pipeline.

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseAuthorization();

app.MapControllers();

app.Run();
