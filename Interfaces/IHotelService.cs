using HotelBooking.Models;

namespace HotelBooking.Interfaces
{
    public interface IHotelService
    {
        Task<List<Hotel>> GetAll();
        Task<Hotel> Create(Hotel hotel);
    }
}
