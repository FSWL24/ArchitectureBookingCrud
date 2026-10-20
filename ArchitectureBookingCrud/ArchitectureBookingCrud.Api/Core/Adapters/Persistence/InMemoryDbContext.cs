using ArchitectureBookingCrud.Api.Core.Domain.Models;

namespace ArchitectureBookingCrud.Api.Core.Adapters.Persistence;

public sealed class InMemoryDbContext
{
    private readonly object _gate = new();
    private readonly Dictionary<int, Booking> _bookings = [];
    private int _nextId = 1;

    public Booking Add(Booking booking)
    {
        lock (_gate)
        {
            var storedBooking = Copy(booking);
            storedBooking.Id = _nextId++;
            _bookings.Add(storedBooking.Id, storedBooking);
            return Copy(storedBooking);
        }
    }

    public Booking? GetById(int id)
    {
        lock (_gate)
        {
            return _bookings.TryGetValue(id, out var booking) ? Copy(booking) : null;
        }
    }

    public IReadOnlyList<Booking> GetBetween(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        lock (_gate)
        {
            return _bookings.Values
                .Where(booking => booking.CheckInDate < endDate && booking.CheckOutDate > startDate)
                .OrderBy(booking => booking.CheckInDate)
                .Select(Copy)
                .ToList();
        }
    }

    public Booking? Update(Booking booking)
    {
        lock (_gate)
        {
            if (!_bookings.TryGetValue(booking.Id, out var storedBooking))
            {
                return null;
            }

            storedBooking.CustomerName = booking.CustomerName;
            storedBooking.CheckInDate = booking.CheckInDate;
            storedBooking.CheckOutDate = booking.CheckOutDate;
            storedBooking.RoomType = booking.RoomType;
            return Copy(storedBooking);
        }
    }

    public bool Cancel(int id)
    {
        lock (_gate)
        {
            if (!_bookings.TryGetValue(id, out var booking))
            {
                return false;
            }

            booking.IsCancelled = true;
            return true;
        }
    }

    private static Booking Copy(Booking booking) => new()
    {
        Id = booking.Id,
        CustomerName = booking.CustomerName,
        CheckInDate = booking.CheckInDate,
        CheckOutDate = booking.CheckOutDate,
        RoomType = booking.RoomType,
        IsCancelled = booking.IsCancelled
    };
}
