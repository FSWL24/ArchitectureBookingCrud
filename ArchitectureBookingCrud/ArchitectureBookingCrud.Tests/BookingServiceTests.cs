using ArchitectureBookingCrud.Api.Core.Adapters.Persistence;
using ArchitectureBookingCrud.Api.Core.Domain.Models;
using ArchitectureBookingCrud.Api.Core.Domain.Services;

public sealed class BookingServiceTests
{
    [Fact]
    public async Task CreateBooking_AssignsUniqueIds()
    {
        var service = CreateService();

        var first = await service.CreateBookingAsync(ValidBooking(), CancellationToken.None);
        var second = await service.CreateBookingAsync(ValidBooking(), CancellationToken.None);

        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public async Task GetBookingsBetween_ReturnsBookingsThatOverlapRange()
    {
        var service = CreateService();
        await service.CreateBookingAsync(ValidBooking(), CancellationToken.None);

        var bookings = await service.GetBookingsBetweenAsync(
            DateTimeOffset.Parse("2026-07-11T00:00:00Z"),
            DateTimeOffset.Parse("2026-07-13T00:00:00Z"),
            CancellationToken.None);

        Assert.Single(bookings);
    }

    [Fact]
    public async Task UpdateBooking_ReturnsNullForUnknownId()
    {
        var service = CreateService();
        var booking = ValidBooking();
        booking.Id = 404;

        var updated = await service.UpdateBookingAsync(booking, CancellationToken.None);

        Assert.Null(updated);
    }

    [Fact]
    public async Task UpdateBooking_DoesNotUncancelCancelledBooking()
    {
        var service = CreateService();
        var booking = await service.CreateBookingAsync(ValidBooking(), CancellationToken.None);
        await service.CancelBookingAsync(booking.Id, CancellationToken.None);
        booking.CustomerName = "Updated Customer";

        var updated = await service.UpdateBookingAsync(booking, CancellationToken.None);

        Assert.True(updated!.IsCancelled);
        Assert.Equal("Updated Customer", updated.CustomerName);
    }

    [Fact]
    public async Task CancelBooking_PreservesBookingAndMarksItCancelled()
    {
        var service = CreateService();
        var booking = await service.CreateBookingAsync(ValidBooking(), CancellationToken.None);

        var cancelled = await service.CancelBookingAsync(booking.Id, CancellationToken.None);
        var retrieved = await service.GetBookingAsync(booking.Id, CancellationToken.None);

        Assert.True(cancelled);
        Assert.True(retrieved!.IsCancelled);
    }

    [Fact]
    public async Task CreateBooking_RejectsInvalidDateRange()
    {
        var service = CreateService();
        var booking = ValidBooking();
        booking.CheckOutDate = booking.CheckInDate;

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateBookingAsync(booking, CancellationToken.None));
    }

    private static BookingService CreateService() =>
        new(new BookingRepository(new InMemoryDbContext()));

    private static Booking ValidBooking() => new()
    {
        CustomerName = "Avery Jones",
        CheckInDate = DateTimeOffset.Parse("2026-07-10T15:00:00-05:00"),
        CheckOutDate = DateTimeOffset.Parse("2026-07-12T11:00:00-05:00"),
        RoomType = "Deluxe"
    };
}
