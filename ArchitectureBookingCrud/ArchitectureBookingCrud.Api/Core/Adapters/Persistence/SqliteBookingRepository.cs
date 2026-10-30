using ArchitectureBookingCrud.Api.Core.Domain.Interfaces;
using ArchitectureBookingCrud.Api.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ArchitectureBookingCrud.Api.Core.Adapters.Persistence;

public sealed class SqliteBookingRepository(BookingSqliteDbContext context) : IBookingRepository
{
    public async Task<Booking> AddAsync(Booking booking, CancellationToken cancellationToken)
    {
        context.Bookings.Add(booking);
        await context.SaveChangesAsync(cancellationToken);
        return booking;
    }

    public Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        context.Bookings.FirstOrDefaultAsync(booking => booking.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetBetweenAsync(
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken) =>
        await context.Bookings
            .Where(booking => booking.CheckInDate < endDate && booking.CheckOutDate > startDate)
            .OrderBy(booking => booking.CheckInDate)
            .ToListAsync(cancellationToken);

    public async Task<Booking?> UpdateAsync(Booking booking, CancellationToken cancellationToken)
    {
        var storedBooking = await context.Bookings.FindAsync([booking.Id], cancellationToken);
        if (storedBooking is null)
        {
            return null;
        }

        storedBooking.CustomerName = booking.CustomerName;
        storedBooking.CheckInDate = booking.CheckInDate;
        storedBooking.CheckOutDate = booking.CheckOutDate;
        storedBooking.RoomType = booking.RoomType;
        await context.SaveChangesAsync(cancellationToken);
        return storedBooking;
    }

    public async Task<bool> CancelAsync(int id, CancellationToken cancellationToken)
    {
        var booking = await context.Bookings.FindAsync([id], cancellationToken);
        if (booking is null)
        {
            return false;
        }

        booking.IsCancelled = true;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var booking = await context.Bookings.FindAsync([id], cancellationToken);
        if (booking is null)
        {
            return false;
        }

        context.Bookings.Remove(booking);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
