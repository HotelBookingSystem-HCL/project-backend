using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        // We store the hashed password, never plain text!
        public string PasswordHash { get; set; } = string.Empty;

        // Role can be "Admin" or "User"
        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation property - one user can have many bookings
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
