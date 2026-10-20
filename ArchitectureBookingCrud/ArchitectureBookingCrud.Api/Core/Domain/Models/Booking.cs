namespace ArchitectureBookingCrud.Api.Core.Domain.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTimeOffset CheckInDate { get; set; }
        public DateTimeOffset CheckOutDate { get; set; }
        public string RoomType { get; set; } = string.Empty;
        public bool IsCancelled { get; set; }
    }
}
