// Repositories/RoomRepository.cs

using HotelBooking.Data;
using HotelBooking.Interfaces;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext _context;

        public RoomRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            return await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);
        }

        public async Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId)
        {
            return await _context.Rooms
                .Include(r => r.Hotel)
                .Where(r => r.HotelId == hotelId && r.IsActive)
                .ToListAsync();
        }

        public async Task<Room> AddAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<Room> UpdateAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return false;

            // Soft delete
            room.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        // Check if a room is free for the given date range
        // A room is NOT available if it has a confirmed/pending booking that overlaps
        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
        {
            var hasConflict = await _context.Bookings
                .AnyAsync(b =>
                    b.RoomId == roomId &&
                    (b.Status == "Confirmed" || b.Status == "Pending") &&
                    b.CheckInDate < checkOut &&   // existing booking starts before our checkout
                    b.CheckOutDate > checkIn);    // existing booking ends after our checkin

            return !hasConflict;
        }
    }
}