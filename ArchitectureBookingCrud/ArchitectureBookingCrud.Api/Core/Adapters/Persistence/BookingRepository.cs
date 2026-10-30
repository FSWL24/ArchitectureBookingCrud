using ArchitectureBookingCrud.Api.Core.Domain.Interfaces;
using ArchitectureBookingCrud.Api.Core.Domain.Models;

namespace ArchitectureBookingCrud.Api.Core.Adapters.Persistence;

public sealed class BookingRepository(InMemoryDbContext context) : IBookingRepository
{
    public Task<Booking> AddAsync(Booking booking, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(context.Add(booking));
    }

    public Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(context.GetById(id));
    }

    public Task<IReadOnlyList<Booking>> GetBetweenAsync(
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(context.GetBetween(startDate, endDate));
    }

    public Task<Booking?> UpdateAsync(Booking booking, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(context.Update(booking));
    }

    public Task<bool> CancelAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(context.Cancel(id));
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(context.Delete(id));
    }
}
