namespace HotelBooking.Models
{
    public class Booking
    {
        public int Id { get; set; }

        // Unique reservation number shown to the user e.g. "HB-20240401-0001"
        public string ReservationNumber { get; set; } = string.Empty;

        // Foreign keys
        public int UserId { get; set; }
        public int RoomId { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        // Number of guests
        public int NumberOfGuests { get; set; }

        // Calculated: (CheckOutDate - CheckInDate).Days * Room.PricePerNight
        public decimal TotalAmount { get; set; }

        // Status: "Pending", "Confirmed", "Cancelled", "Completed"
        public string Status { get; set; } = "Pending";

        // Any special requests from the customer
        public string SpecialRequests { get; set; } = string.Empty;

        // Was a confirmation email sent?
        public bool EmailSent { get; set; } = false;

        // Optional: discount code applied
        public string? PromoCode { get; set; }
        public decimal DiscountAmount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;
        public Room Room { get; set; } = null!;
    }
}
