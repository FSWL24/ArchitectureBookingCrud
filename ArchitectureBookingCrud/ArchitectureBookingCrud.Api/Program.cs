using ArchitectureBookingCrud.Api.Core.Adapters.Persistence;
using ArchitectureBookingCrud.Api.Core.Domain.Interfaces;
using ArchitectureBookingCrud.Api.Core.Domain.Services;

using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Architectural Booking CRUD API", Version = "v1" });
});

builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddSingleton<InMemoryDbContext>();

var app = builder.Build();


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
