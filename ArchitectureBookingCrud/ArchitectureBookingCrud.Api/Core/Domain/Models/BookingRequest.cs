using System.ComponentModel.DataAnnotations;

namespace ArchitectureBookingCrud.Api.Core.Domain.Models;

/// <summary>Booking details supplied when creating or updating a reservation.</summary>
public sealed record BookingRequest : IValidatableObject
{
    /// <summary>Name of the customer who made the booking.</summary>
    [Required, StringLength(100)]
    public string CustomerName { get; init; } = string.Empty;

    /// <summary>Start of the customer's stay, including its UTC offset.</summary>
    public DateTimeOffset CheckInDate { get; init; }

    /// <summary>End of the customer's stay, including its UTC offset.</summary>
    public DateTimeOffset CheckOutDate { get; init; }

    /// <summary>Requested room category.</summary>
    [Required, StringLength(50)]
    public string RoomType { get; init; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckInDate >= CheckOutDate)
        {
            yield return new ValidationResult(
                "Check-in date must be earlier than check-out date.",
                [nameof(CheckInDate), nameof(CheckOutDate)]);
        }
    }
}
