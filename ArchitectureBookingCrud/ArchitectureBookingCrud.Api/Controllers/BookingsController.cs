using ArchitectureBookingCrud.Api.Core.Domain.Interfaces;
using ArchitectureBookingCrud.Api.Core.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArchitectureBookingCrud.Api.Controllers;

[ApiController]
[Route("api/bookings")]
[Produces("application/json")]
public sealed class BookingsController(IBookingService bookingService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<BookingResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingResponse>> CreateBooking(
        BookingRequest request,
        CancellationToken cancellationToken)
    {
        var booking = await bookingService.CreateBookingAsync(ToBooking(request), cancellationToken);
        var response = ToResponse(booking);
        return CreatedAtAction(nameof(GetBooking), new { id = response.Id }, response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<BookingResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingResponse>> GetBooking(
        int id,
        CancellationToken cancellationToken)
    {
        var booking = await bookingService.GetBookingAsync(id, cancellationToken);
        return booking is null ? NotFound() : Ok(ToResponse(booking));
    }

    [HttpGet("between")]
    [ProducesResponseType<IReadOnlyList<BookingResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<BookingResponse>>> GetBookingsBetween(
        [FromQuery] DateTimeOffset startDate,
        [FromQuery] DateTimeOffset endDate,
        CancellationToken cancellationToken)
    {
        if (startDate >= endDate)
        {
            ModelState.AddModelError(nameof(startDate), "Start date must be earlier than end date.");
            return BadRequest(new ValidationProblemDetails(ModelState)
            {
                Status = StatusCodes.Status400BadRequest
            });
        }

        var bookings = await bookingService.GetBookingsBetweenAsync(startDate, endDate, cancellationToken);
        return Ok(bookings.Select(ToResponse).ToList());
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<BookingResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingResponse>> UpdateBooking(
        int id,
        BookingRequest request,
        CancellationToken cancellationToken)
    {
        var booking = ToBooking(request);
        booking.Id = id;

        var updatedBooking = await bookingService.UpdateBookingAsync(booking, cancellationToken);
        return updatedBooking is null ? NotFound() : Ok(ToResponse(updatedBooking));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBooking(int id, CancellationToken cancellationToken)
    {
        var deleted = await bookingService.DeleteBookingAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelBooking(int id, CancellationToken cancellationToken)
    {
        var cancelled = await bookingService.CancelBookingAsync(id, cancellationToken);
        return cancelled ? NoContent() : NotFound();
    }

    private static Booking ToBooking(BookingRequest request) => new()
    {
        CustomerName = request.CustomerName,
        CheckInDate = request.CheckInDate,
        CheckOutDate = request.CheckOutDate,
        RoomType = request.RoomType
    };

    private static BookingResponse ToResponse(Booking booking) => new(
        booking.Id,
        booking.CustomerName,
        booking.CheckInDate,
        booking.CheckOutDate,
        booking.RoomType,
        booking.IsCancelled);
}
