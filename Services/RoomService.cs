// Services/RoomService.cs
// Business logic for room operations

using HotelBooking.DTOs;
using HotelBooking.Interfaces;
using HotelBooking.Models;

namespace HotelBooking.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IHotelRepository _hotelRepository;

        public RoomService(IRoomRepository roomRepository, IHotelRepository hotelRepository)
        {
            _roomRepository = roomRepository;
            _hotelRepository = hotelRepository;
        }

        public async Task<IEnumerable<RoomResponseDto>> GetRoomsByHotelAsync(int hotelId)
        {
            var rooms = await _roomRepository.GetByHotelIdAsync(hotelId);
            return rooms.Select(MapToResponseDto);
        }

        public async Task<RoomResponseDto?> GetRoomByIdAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            return room == null ? null : MapToResponseDto(room);
        }

        public async Task<IEnumerable<RoomResponseDto>> SearchAvailableRoomsAsync(RoomSearchDto searchDto)
        {
            var rooms = await _roomRepository.GetByHotelIdAsync(searchDto.HotelId);

            // Only show available rooms
            var availableRooms = new List<Room>();
            foreach (var room in rooms.Where(r => r.IsAvailable && r.IsActive))
            {
                // Check the dates are actually free
                var isAvailable = await _roomRepository.IsRoomAvailableAsync(
                    room.Id, searchDto.CheckInDate, searchDto.CheckOutDate);

                if (isAvailable)
                    availableRooms.Add(room);
            }

            // Filter by number of guests
            var filtered = availableRooms
                .Where(r => r.MaxOccupancy >= searchDto.NumberOfGuests);

            // Filter by price
            if (searchDto.MaxPricePerNight.HasValue)
                filtered = filtered.Where(r => r.PricePerNight <= searchDto.MaxPricePerNight.Value);

            // Filter by room type
            if (!string.IsNullOrWhiteSpace(searchDto.RoomType))
                filtered = filtered.Where(r =>
                    r.RoomType.Equals(searchDto.RoomType, StringComparison.OrdinalIgnoreCase));

            return filtered.Select(MapToResponseDto);
        }

        public async Task<RoomResponseDto> CreateRoomAsync(int hotelId, RoomDto roomDto)
        {
            var room = new Room
            {
                HotelId = hotelId,
                RoomNumber = roomDto.RoomNumber,
                RoomType = roomDto.RoomType,
                Description = roomDto.Description,
                PricePerNight = roomDto.PricePerNight,
                MaxOccupancy = roomDto.MaxOccupancy,
                Features = roomDto.Features,
                ImageUrl = roomDto.ImageUrl,
                IsAvailable = roomDto.IsAvailable,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _roomRepository.AddAsync(room);

            // Reload with Hotel navigation property
            var reloaded = await _roomRepository.GetByIdAsync(created.Id);
            return MapToResponseDto(reloaded!);
        }

        public async Task<RoomResponseDto?> UpdateRoomAsync(int id, RoomDto roomDto)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return null;

            room.RoomNumber = roomDto.RoomNumber;
            room.RoomType = roomDto.RoomType;
            room.Description = roomDto.Description;
            room.PricePerNight = roomDto.PricePerNight;
            room.MaxOccupancy = roomDto.MaxOccupancy;
            room.Features = roomDto.Features;
            room.ImageUrl = roomDto.ImageUrl;
            room.IsAvailable = roomDto.IsAvailable;

            var updated = await _roomRepository.UpdateAsync(room);
            return MapToResponseDto(updated);
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            return await _roomRepository.DeleteAsync(id);
        }

        private static RoomResponseDto MapToResponseDto(Room room)
        {
            return new RoomResponseDto
            {
                Id = room.Id,
                HotelId = room.HotelId,
                HotelName = room.Hotel?.Name ?? "",
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                Description = room.Description,
                PricePerNight = room.PricePerNight,
                MaxOccupancy = room.MaxOccupancy,
                Features = room.Features,
                ImageUrl = room.ImageUrl,
                IsAvailable = room.IsAvailable
            };
        }
    }
}