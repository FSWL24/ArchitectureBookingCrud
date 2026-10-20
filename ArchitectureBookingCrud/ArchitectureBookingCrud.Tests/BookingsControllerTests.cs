using ArchitectureBookingCrud.Api.Controllers;
using ArchitectureBookingCrud.Api.Core.Domain.Interfaces;
using ArchitectureBookingCrud.Api.Core.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

public sealed class BookingsControllerTests
{
    private static readonly CancellationToken CancellationToken = CancellationToken.None;

    [Fact]
    public async Task CreateBooking_ReturnsCreatedResourceAndLocation()
    {
        var service = new Mock<IBookingService>();
        service.Setup(x => x.CreateBookingAsync(It.IsAny<Booking>(), CancellationToken))
            .ReturnsAsync((Booking booking, CancellationToken _) =>
            {
                booking.Id = 1;
                return booking;
            });
        var controller = new BookingsController(service.Object);

        var result = await controller.CreateBooking(ValidRequest(), CancellationToken);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(BookingsController.GetBooking), created.ActionName);
        Assert.Equal(1, created.RouteValues!["id"]);
        Assert.Equal(1, Assert.IsType<BookingResponse>(created.Value).Id);
    }

    [Fact]
    public async Task GetBooking_ReturnsBookingWhenFound()
    {
        var booking = ValidBooking(4);
        var controller = CreateController(service =>
            service.Setup(x => x.GetBookingAsync(4, CancellationToken)).ReturnsAsync(booking));

        var result = await controller.GetBooking(4, CancellationToken);

        var response = Assert.IsType<BookingResponse>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal(booking.Id, response.Id);
    }

    [Fact]
    public async Task GetBooking_ReturnsNotFoundWhenMissing()
    {
        var controller = CreateController(service =>
            service.Setup(x => x.GetBookingAsync(404, CancellationToken)).ReturnsAsync((Booking?)null));

        var result = await controller.GetBooking(404, CancellationToken);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetBookingsBetween_ReturnsOverlappingBookings()
    {
        var booking = ValidBooking(1);
        var controller = CreateController(service =>
            service.Setup(x => x.GetBookingsBetweenAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    CancellationToken))
                .ReturnsAsync((IReadOnlyList<Booking>)[booking]));

        var result = await controller.GetBookingsBetween(
            booking.CheckInDate,
            booking.CheckOutDate,
            CancellationToken);

        var responses = Assert.IsAssignableFrom<IReadOnlyList<BookingResponse>>(
            Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Single(responses);
    }

    [Fact]
    public async Task GetBookingsBetween_ReturnsValidationProblemForInvalidRange()
    {
        var controller = CreateController(_ => { });
        var date = DateTimeOffset.Parse("2026-01-01T00:00:00Z");

        var result = await controller.GetBookingsBetween(date, date, CancellationToken);

        var problem = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, problem.StatusCode);
        Assert.IsType<ValidationProblemDetails>(problem.Value);
    }

    [Fact]
    public async Task UpdateBooking_UsesRouteIdAndReturnsUpdatedBooking()
    {
        var controller = CreateController(service =>
            service.Setup(x => x.UpdateBookingAsync(
                    It.Is<Booking>(booking => booking.Id == 23),
                    CancellationToken))
                .ReturnsAsync((Booking booking, CancellationToken _) => booking));

        var result = await controller.UpdateBooking(23, ValidRequest(), CancellationToken);

        var response = Assert.IsType<BookingResponse>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal(23, response.Id);
    }

    [Fact]
    public async Task UpdateBooking_ReturnsNotFoundWhenBookingDoesNotExist()
    {
        var controller = CreateController(service =>
            service.Setup(x => x.UpdateBookingAsync(It.IsAny<Booking>(), CancellationToken))
                .ReturnsAsync((Booking?)null));

        var result = await controller.UpdateBooking(404, ValidRequest(), CancellationToken);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CancelBooking_ReturnsNoContentWhenBookingExists()
    {
        var controller = CreateController(service =>
            service.Setup(x => x.CancelBookingAsync(7, CancellationToken)).ReturnsAsync(true));

        var result = await controller.CancelBooking(7, CancellationToken);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task CancelBooking_ReturnsNotFoundWhenBookingDoesNotExist()
    {
        var controller = CreateController(service =>
            service.Setup(x => x.CancelBookingAsync(404, CancellationToken)).ReturnsAsync(false));

        var result = await controller.CancelBooking(404, CancellationToken);

        Assert.IsType<NotFoundResult>(result);
    }

    private static BookingsController CreateController(Action<Mock<IBookingService>> setup)
    {
        var service = new Mock<IBookingService>();
        setup(service);
        return new BookingsController(service.Object);
    }

    private static BookingRequest ValidRequest() => new()
    {
        CustomerName = "Avery Jones",
        CheckInDate = DateTimeOffset.Parse("2026-07-10T15:00:00-05:00"),
        CheckOutDate = DateTimeOffset.Parse("2026-07-12T11:00:00-05:00"),
        RoomType = "Deluxe"
    };

    private static Booking ValidBooking(int id) => new()
    {
        Id = id,
        CustomerName = "Avery Jones",
        CheckInDate = DateTimeOffset.Parse("2026-07-10T15:00:00-05:00"),
        CheckOutDate = DateTimeOffset.Parse("2026-07-12T11:00:00-05:00"),
        RoomType = "Deluxe"
    };
}
