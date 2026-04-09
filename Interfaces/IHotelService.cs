// Interfaces/IHotelService.cs

using HotelBooking.DTOs;

namespace HotelBooking.Interfaces
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelResponseDto>> GetAllHotelsAsync();
        Task<HotelResponseDto?> GetHotelByIdAsync(int id);
        Task<IEnumerable<HotelResponseDto>> SearchHotelsAsync(HotelSearchDto searchDto);
        Task<HotelResponseDto> CreateHotelAsync(HotelDto hotelDto);
        Task<HotelResponseDto?> UpdateHotelAsync(int id, HotelDto hotelDto);
        Task<bool> DeleteHotelAsync(int id);
    }
}