using System.Text;
using System.Security.Cryptography;

namespace HotelBooking.Helpers
{
    public class PasswordHasher
    {


        // Hash a plain-text password
        // Example: "MyPassword123" → "$2a$11$randomsalt..."
        public static string Hash(string password)
        {
            // BCrypt.Net-Next library handles this
            // WorkFactor 11 means it does 2^11 = 2048 iterations (slower = more secure)
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
        }

        // Verify a plain-text password against a stored hash
        // Returns true if they match
        public static bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }


}

