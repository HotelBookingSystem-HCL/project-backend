using HotelBooking.Dto.Auth;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var token = await _service.Register(dto);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Username = dto.Username,
            Role = "User"
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _service.Login(dto);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Username = dto.Username
        });
    }
}