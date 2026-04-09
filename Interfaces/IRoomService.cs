// Interfaces/IRoomService.cs

using HotelBooking.DTOs;

namespace HotelBooking.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomResponseDto>> GetRoomsByHotelAsync(int hotelId);
        Task<RoomResponseDto?> GetRoomByIdAsync(int id);
        Task<IEnumerable<RoomResponseDto>> SearchAvailableRoomsAsync(RoomSearchDto searchDto);
        Task<RoomResponseDto> CreateRoomAsync(int hotelId, RoomDto roomDto);
        Task<RoomResponseDto?> UpdateRoomAsync(int id, RoomDto roomDto);
        Task<bool> DeleteRoomAsync(int id);
    }
}