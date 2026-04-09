using HotelBooking.Data;
using HotelBooking.Interfaces;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
namespace HotelBooking.Services;

public class HotelService : IHotelService
{
    private readonly AppDbContext _db;

    public HotelService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Hotel>> GetAll()
    {
        return await _db.Hotels
            .Include(h => h.Rooms)
            .ToListAsync();
    }

    public async Task<Hotel> Create(Hotel hotel)
    {
        _db.Hotels.Add(hotel);
        await _db.SaveChangesAsync();

        Log.Information($"Hotel created: {hotel.Name}");

        return hotel;
    }
}