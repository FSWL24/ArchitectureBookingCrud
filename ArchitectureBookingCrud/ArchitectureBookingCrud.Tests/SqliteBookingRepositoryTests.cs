using ArchitectureBookingCrud.Api.Core.Adapters.Persistence;
using ArchitectureBookingCrud.Api.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

public sealed class SqliteBookingRepositoryTests
{
    [Fact]
    public async Task AddAsync_PersistsBookingAcrossContextInstances()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.db");
        var options = new DbContextOptionsBuilder<BookingSqliteDbContext>()
            .UseSqlite($"Data Source={databasePath}")
            .Options;

        try
        {
            int bookingId;
            await using (var context = new BookingSqliteDbContext(options))
            {
                await context.Database.EnsureCreatedAsync();
                var repository = new SqliteBookingRepository(context);
                var created = await repository.AddAsync(ValidBooking(), CancellationToken.None);
                bookingId = created.Id;
            }

            await using (var context = new BookingSqliteDbContext(options))
            {
                var repository = new SqliteBookingRepository(context);
                var retrieved = await repository.GetByIdAsync(bookingId, CancellationToken.None);

                Assert.NotNull(retrieved);
                Assert.Equal("Avery Jones", retrieved.CustomerName);
                Assert.Equal(ValidBooking().CheckInDate, retrieved.CheckInDate);
            }
        }
        finally
        {
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }

    [Fact]
    public async Task GetBetweenAsync_ReturnsOverlappingBookingsInCheckInOrder()
    {
        await using var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<BookingSqliteDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var context = new BookingSqliteDbContext(options);
        await context.Database.EnsureCreatedAsync();
        var repository = new SqliteBookingRepository(context);

        await repository.AddAsync(ValidBooking(), CancellationToken.None);
        var earlierBooking = ValidBooking();
        earlierBooking.CheckInDate = DateTimeOffset.Parse("2026-07-09T15:00:00-05:00");
        await repository.AddAsync(earlierBooking, CancellationToken.None);

        var bookings = await repository.GetBetweenAsync(
            DateTimeOffset.Parse("2026-07-11T00:00:00Z"),
            DateTimeOffset.Parse("2026-07-13T00:00:00Z"),
            CancellationToken.None);

        Assert.Equal(2, bookings.Count);
        Assert.True(bookings[0].CheckInDate < bookings[1].CheckInDate);
    }

    [Fact]
    public async Task DeleteAsync_RemovesBookingFromDatabase()
    {
        await using var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<BookingSqliteDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var context = new BookingSqliteDbContext(options);
        await context.Database.EnsureCreatedAsync();
        var repository = new SqliteBookingRepository(context);
        var booking = await repository.AddAsync(ValidBooking(), CancellationToken.None);

        var deleted = await repository.DeleteAsync(booking.Id, CancellationToken.None);
        var retrieved = await repository.GetByIdAsync(booking.Id, CancellationToken.None);

        Assert.True(deleted);
        Assert.Null(retrieved);
    }

    private static Booking ValidBooking() => new()
    {
        CustomerName = "Avery Jones",
        CheckInDate = DateTimeOffset.Parse("2026-07-10T15:00:00-05:00"),
        CheckOutDate = DateTimeOffset.Parse("2026-07-12T11:00:00-05:00"),
        RoomType = "Deluxe"
    };
}
