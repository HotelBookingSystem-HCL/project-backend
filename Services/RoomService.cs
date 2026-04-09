using HotelBooking.Data;
using HotelBooking.Interfaces;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
namespace HotelBooking.Services;
public class RoomService : IRoomService
{
    private readonly AppDbContext _db;

    public RoomService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Room>> GetAll()
    {
        return await _db.Rooms
            .Include(r => r.Hotel)
            .ToListAsync();
    }

    public async Task<Room> Add(Room room)
    {
        var hotelExists = await _db.Hotels.AnyAsync(h => h.Id == room.HotelId);

        if (!hotelExists)
            throw new Exception("Invalid Hotel ID");

        _db.Rooms.Add(room);
        await _db.SaveChangesAsync();

        Log.Information($"Room added: {room.Category}");

        return room;
    }

    public async Task<List<Room>> Search(string? location, decimal? minPrice, decimal? maxPrice)
    {
        var query = _db.Rooms.Include(r => r.Hotel).AsQueryable();

        if (!string.IsNullOrEmpty(location))
            query = query.Where(r => r.Hotel.Location.Contains(location));

        if (minPrice.HasValue)
            query = query.Where(r => r.Price >= minPrice);

        if (maxPrice.HasValue)
            query = query.Where(r => r.Price <= maxPrice);

        return await query.ToListAsync();
    }
}