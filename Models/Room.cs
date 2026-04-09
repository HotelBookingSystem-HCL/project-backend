namespace HotelBooking.Models
{
    public class Room
    {
        public int Id { get; set; }

        // Foreign key to Hotel
        public int HotelId { get; set; }

        public string RoomNumber { get; set; } = string.Empty;

        // e.g. "Single", "Double", "Suite", "Deluxe"
        public string RoomType { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Price per night
        public decimal PricePerNight { get; set; }

        // Maximum number of guests allowed
        public int MaxOccupancy { get; set; }

        // Comma-separated features e.g. "AC, TV, Mini-bar"
        public string Features { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        // Is this room currently available for booking?
        public bool IsAvailable { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Hotel Hotel { get; set; } = null!;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
