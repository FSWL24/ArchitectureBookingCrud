using ArchitectureBookingCrud.Api.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ArchitectureBookingCrud.Api.Core.Adapters.Persistence;

public sealed class BookingSqliteDbContext(DbContextOptions<BookingSqliteDbContext> options) : DbContext(options)
{
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var dateTimeOffsetConverter = new ValueConverter<DateTimeOffset, long>(
            value => value.UtcTicks,
            value => new DateTimeOffset(value, TimeSpan.Zero));
        var bookings = modelBuilder.Entity<Booking>();
        bookings.HasKey(booking => booking.Id);
        bookings.Property(booking => booking.CustomerName).IsRequired();
        bookings.Property(booking => booking.RoomType).IsRequired();
        bookings.Property(booking => booking.CheckInDate).HasConversion(dateTimeOffsetConverter);
        bookings.Property(booking => booking.CheckOutDate).HasConversion(dateTimeOffsetConverter);
    }
}
