using System.Text;
using System.Security.Cryptography;

namespace HotelBooking.Helpers
{
    public class PasswordHasher
    {

        public static string Hash(string password)
        {
            return Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        }

        public static bool Verify(string password, string hash)
        {
            return Hash(password) == hash;
        }
    }

    }