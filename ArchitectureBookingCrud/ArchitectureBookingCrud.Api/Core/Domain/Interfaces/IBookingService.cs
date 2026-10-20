using ArchitectureBookingCrud.Api.Core.Domain.Models;

namespace ArchitectureBookingCrud.Api.Core.Domain.Interfaces;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(Booking booking, CancellationToken cancellationToken);
    Task<Booking?> GetBookingAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Booking>> GetBookingsBetweenAsync(
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken);
    Task<Booking?> UpdateBookingAsync(Booking booking, CancellationToken cancellationToken);
    Task<bool> CancelBookingAsync(int id, CancellationToken cancellationToken);
}
