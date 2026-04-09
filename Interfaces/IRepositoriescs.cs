// Interfaces/IUserRepository.cs
// Repository interfaces abstract away the database access logic

using HotelBooking.Models;

namespace HotelBooking.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<bool> EmailExistsAsync(string email);
    }

    public interface IHotelRepository
    {
        Task<Hotel?> GetByIdAsync(int id);
        Task<IEnumerable<Hotel>> GetAllAsync();
        Task<Hotel> AddAsync(Hotel hotel);
        Task<Hotel> UpdateAsync(Hotel hotel);
        Task<bool> DeleteAsync(int id);
    }

    public interface IRoomRepository
    {
        Task<Room?> GetByIdAsync(int id);
        Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId);
        Task<Room> AddAsync(Room room);
        Task<Room> UpdateAsync(Room room);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);
    }

    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(int id);
        Task<IEnumerable<Booking>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<Booking> AddAsync(Booking booking);
        Task<Booking> UpdateAsync(Booking booking);
        Task<string> GenerateReservationNumberAsync();
    }
}