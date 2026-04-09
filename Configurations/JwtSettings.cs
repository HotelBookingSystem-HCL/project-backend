using HotelBooking.Models;
using Microsoft.AspNetCore.DataProtection;

namespace HotelBooking.Configurations
{
    public class JwtSettings
    {
        
        public string SecretKey { get; set; } = string.Empty;

        // Who issued the token (usually your app name or domain)
        public string Issuer { get; set; } = string.Empty;

        // Who the token is intended for
        public string Audience { get; set; } = string.Empty;

        // How many minutes until the token expires
        public int ExpiryMinutes { get; set; } = 60;
    }
}
