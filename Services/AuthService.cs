namespace HotelBooking.Services;

using HotelBooking.Data;
using HotelBooking.Dto.Auth;
using HotelBooking.Helpers;
using HotelBooking.Interfaces;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly JwtHelper _jwt;

    public AuthService(AppDbContext db, JwtHelper jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<string> Register(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(x => x.Username == dto.Username))
            throw new Exception("Username already exists");

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = PasswordHasher.Hash(dto.Password),
            Role = "User"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        Log.Information("User registered: {Username}", user.Username);

        return _jwt.GenerateToken(user);
    }

    public async Task<string> Login(LoginDto dto)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(x => x.Username == dto.Username);

        if (user == null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
        {
            Log.Warning("Invalid login attempt");
            throw new Exception("Invalid credentials");
        }

        Log.Information("User logged in: {Username}", user.Username);

        return _jwt.GenerateToken(user);
    }
}