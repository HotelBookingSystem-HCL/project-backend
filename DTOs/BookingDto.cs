// DTOs/BookingDto.cs
// DTOs for booking operations

namespace HotelBooking.DTOs
{
    // What the user sends when making a new booking
    public class BookingDto
    {
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public string SpecialRequests { get; set; } = string.Empty;
        public string? PromoCode { get; set; }
    }

    // What we send back to the user (booking details)
    public class BookingResponseDto
    {
        public int Id { get; set; }
        public string ReservationNumber { get; set; } = string.Empty;
        public string HotelName { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string SpecialRequests { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // For admin to update booking status
    public class UpdateBookingStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }
}