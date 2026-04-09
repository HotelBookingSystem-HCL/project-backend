namespace HotelBooking.DTOs
{
    public class RegisterDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        // Optional: allow specifying role (only admins should be able to set "Admin")
        public string Role { get; set; } = "User";
    }
}
