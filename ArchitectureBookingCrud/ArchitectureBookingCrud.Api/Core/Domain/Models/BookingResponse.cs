namespace ArchitectureBookingCrud.Api.Core.Domain.Models;

/// <summary>Booking details returned by the API.</summary>
public sealed record BookingResponse(
    int Id,
    string CustomerName,
    DateTimeOffset CheckInDate,
    DateTimeOffset CheckOutDate,
    string RoomType,
    bool IsCancelled);
