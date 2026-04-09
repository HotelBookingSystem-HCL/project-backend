// Services/AuthService.cs
// Handles user registration and login logic

using HotelBooking.DTOs;
using HotelBooking.Helpers;
using HotelBooking.Interfaces;
using HotelBooking.Models;

namespace HotelBooking.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthService(IUserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            // 1. Find user by email
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);

            // 2. Check user exists and is active
            if (user == null || !user.IsActive)
                return null;

            // 3. Verify password
            if (!PasswordHasher.Verify(loginDto.Password, user.PasswordHash))
                return null;

            // 4. Generate JWT token
            var token = _jwtHelper.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                UserId = user.Id
            };
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            // 1. Check if email already exists
            if (await _userRepository.EmailExistsAsync(registerDto.Email))
                return null; // Email taken

            // 2. Create new user (always register as "User" role for security)
            var newUser = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email.ToLower().Trim(),
                PasswordHash = PasswordHasher.Hash(registerDto.Password),
                Role = "User", // New registrations are always "User"
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // 3. Save to database
            var savedUser = await _userRepository.AddAsync(newUser);

            // 4. Generate token so user is auto-logged in after register
            var token = _jwtHelper.GenerateToken(savedUser);

            return new AuthResponseDto
            {
                Token = token,
                FullName = savedUser.FullName,
                Email = savedUser.Email,
                Role = savedUser.Role,
                UserId = savedUser.Id
            };
        }
    }
}