using HotelBooking.Models;

namespace HotelBooking.Interfaces
{
    public interface IRoomService
    {
        Task<List<Room>> GetAll();
        Task<Room> Add(Room room);
        Task<List<Room>> Search(string? location, decimal? minPrice, decimal? maxPrice);
    }
}

