namespace HotelBooking.DTOs
{
    public class HotelDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int StarRating { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Amenities { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    // Used when user searches for hotels
    public class HotelSearchDto
    {
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? MinStarRating { get; set; }
        public decimal? MaxPricePerNight { get; set; }
        public string? Amenity { get; set; }
    }

    // What we return to the client for a hotel
    public class HotelResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int StarRating { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Amenities { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        public decimal? LowestPrice { get; set; }
    }

    // Used when user manages their own profile
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    // Admin: update user details
    public class UpdateUserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}

