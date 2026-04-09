using HotelBooking.Data;
using HotelBooking.Dto.Booking;
using HotelBooking.Interfaces;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
namespace HotelBooking.Services;

public class BookingService : IBookingService
{
    private readonly AppDbContext _db;
    private readonly IEmailService _email;

    public BookingService(AppDbContext db, IEmailService email)
    {
        _db = db;
        _email = email;
    }

    public async Task<string> Book(int userId, CreateBookingDto dto)
    {
        var room = await _db.Rooms
            .Include(r => r.Hotel)
            .FirstOrDefaultAsync(r => r.Id == dto.RoomId);

        if (room == null || !room.IsAvailable)
        {
            Log.Warning("Room not available");
            throw new Exception("Room not available");
        }

        room.IsAvailable = false;

        var booking = new Booking
        {
            UserId = userId,
            RoomId = dto.RoomId,
            FromDate = dto.FromDate,
            ToDate = dto.ToDate
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        // 📧 Send Email
        await _email.SendEmail(
            "user@email.com",
            "Booking Confirmation",
            $"Booking ID: {booking.Id}, Hotel: {room.Hotel.Name}"
        );

        Log.Information($"Booking created: {booking.Id}");

        return $"Booking Confirmed. ID: {booking.Id}";
    }

    public async Task<List<BookingHistoryDto>> GetUserBookings(int userId)
    {
        return await _db.Bookings
            .Where(b => b.UserId == userId)
            .Join(_db.Rooms,
                b => b.RoomId,
                r => r.Id,
                (b, r) => new { b, r })
            .Join(_db.Hotels,
                br => br.r.HotelId,
                h => h.Id,
                (br, h) => new BookingHistoryDto
                {
                    BookingId = br.b.Id,
                    HotelName = h.Name,
                    RoomCategory = br.r.Category,
                    FromDate = br.b.FromDate,
                    ToDate = br.b.ToDate,
                    Price = br.r.Price,
                    Status = br.b.Status
                })
            .ToListAsync();
    }
}