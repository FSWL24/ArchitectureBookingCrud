using ArchitectureBookingCrud.Api.Core.Domain.Interfaces;
using ArchitectureBookingCrud.Api.Core.Domain.Models;

namespace ArchitectureBookingCrud.Api.Core.Domain.Services;

public sealed class BookingService(IBookingRepository bookingRepository) : IBookingService
{
    public Task<Booking> CreateBookingAsync(Booking booking, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(booking);
        ValidateBooking(booking);
        return bookingRepository.AddAsync(booking, cancellationToken);
    }

    public Task<Booking?> GetBookingAsync(int id, CancellationToken cancellationToken) =>
        bookingRepository.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyList<Booking>> GetBookingsBetweenAsync(
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken)
    {
        if (startDate >= endDate)
        {
            throw new ArgumentException("The start date must be earlier than the end date.");
        }

        return bookingRepository.GetBetweenAsync(startDate, endDate, cancellationToken);
    }

    public Task<Booking?> UpdateBookingAsync(Booking booking, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(booking);
        ValidateBooking(booking);
        return bookingRepository.UpdateAsync(booking, cancellationToken);
    }

    public Task<bool> CancelBookingAsync(int id, CancellationToken cancellationToken) =>
        bookingRepository.CancelAsync(id, cancellationToken);

    public Task<bool> DeleteBookingAsync(int id, CancellationToken cancellationToken) =>
        bookingRepository.DeleteAsync(id, cancellationToken);

    private static void ValidateBooking(Booking booking)
    {
        if (string.IsNullOrWhiteSpace(booking.CustomerName))
        {
            throw new ArgumentException("Customer name is required.", nameof(booking));
        }

        if (string.IsNullOrWhiteSpace(booking.RoomType))
        {
            throw new ArgumentException("Room type is required.", nameof(booking));
        }

        if (booking.CheckInDate >= booking.CheckOutDate)
        {
            throw new ArgumentException("Check-in date must be earlier than check-out date.", nameof(booking));
        }
    }
}
