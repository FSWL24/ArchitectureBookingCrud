using ArchitectureBookingCrud.Api.Core.Domain.Interfaces;

namespace ArchitectureBookingCrud.Api.Core.Adapters.Persistence;

public sealed class BookingRepositoryFactory(
    IConfiguration configuration,
    IServiceProvider serviceProvider)
{
    public IBookingRepository Create() =>
        (configuration["Persistence:Provider"] ?? "InMemory").ToUpperInvariant() switch
        {
            "INMEMORY" => serviceProvider.GetRequiredService<BookingRepository>(),
            "SQLITE" => serviceProvider.GetRequiredService<SqliteBookingRepository>(),
            var provider => throw new InvalidOperationException(
                $"Unsupported persistence provider '{provider}'. Use 'InMemory' or 'SQLite'.")
        };
}
