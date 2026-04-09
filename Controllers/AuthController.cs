// Controllers/AuthController.cs
// Handles user registration and login
// No [Authorize] here - these endpoints are public (anyone can call them)

using HotelBooking.DTOs;
using HotelBooking.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route = api/auth
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST api/auth/register
        // Body: { "fullName": "John", "email": "john@example.com", "password": "Pass@123" }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(registerDto);

            if (result == null)
                return BadRequest(new { Message = "Email is already registered." });

            return Ok(result);
        }

        // POST api/auth/login
        // Body: { "email": "john@example.com", "password": "Pass@123" }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
                return Unauthorized(new { Message = "Invalid email or password." });

            return Ok(result);
        }
    }
}