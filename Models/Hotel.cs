namespace HotelBooking.Models
{
    public class Hotel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        // Star rating: 1 to 5
        public int StarRating { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        // Comma-separated list of amenities e.g. "Pool, Gym, WiFi"
        public string Amenities { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property - one hotel has many rooms
        public ICollection<Room> Rooms { get; set; } = new List<Room>();

    }
}
