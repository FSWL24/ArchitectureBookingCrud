using ArchitectureBookingCrud.Api.Core.Domain.Models;

namespace ArchitectureBookingCrud.Api.Core.Domain.Interfaces;

public interface IBookingRepository
{
    Task<Booking> AddAsync(Booking booking, CancellationToken cancellationToken);
    Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Booking>> GetBetweenAsync(
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken);
    Task<Booking?> UpdateAsync(Booking booking, CancellationToken cancellationToken);
    Task<bool> CancelAsync(int id, CancellationToken cancellationToken);
}
