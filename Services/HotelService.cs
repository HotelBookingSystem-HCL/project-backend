// Services/HotelService.cs
// Business logic for hotel operations

using HotelBooking.DTOs;
using HotelBooking.Interfaces;
using HotelBooking.Models;

namespace HotelBooking.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;

        public HotelService(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        public async Task<IEnumerable<HotelResponseDto>> GetAllHotelsAsync()
        {
            var hotels = await _hotelRepository.GetAllAsync();
            return hotels.Select(MapToResponseDto);
        }

        public async Task<HotelResponseDto?> GetHotelByIdAsync(int id)
        {
            var hotel = await _hotelRepository.GetByIdAsync(id);
            return hotel == null ? null : MapToResponseDto(hotel);
        }

        public async Task<IEnumerable<HotelResponseDto>> SearchHotelsAsync(HotelSearchDto searchDto)
        {
            var hotels = await _hotelRepository.GetAllAsync();

            // Filter by city (case-insensitive)
            if (!string.IsNullOrWhiteSpace(searchDto.City))
                hotels = hotels.Where(h =>
                    h.City.Contains(searchDto.City, StringComparison.OrdinalIgnoreCase));

            // Filter by country
            if (!string.IsNullOrWhiteSpace(searchDto.Country))
                hotels = hotels.Where(h =>
                    h.Country.Contains(searchDto.Country, StringComparison.OrdinalIgnoreCase));

            // Filter by minimum star rating
            if (searchDto.MinStarRating.HasValue)
                hotels = hotels.Where(h => h.StarRating >= searchDto.MinStarRating.Value);

            // Filter by amenity (e.g. "Pool")
            if (!string.IsNullOrWhiteSpace(searchDto.Amenity))
                hotels = hotels.Where(h =>
                    h.Amenities.Contains(searchDto.Amenity, StringComparison.OrdinalIgnoreCase));

            // Filter by max price (check if any room is within budget)
            if (searchDto.MaxPricePerNight.HasValue)
                hotels = hotels.Where(h =>
                    h.Rooms.Any(r => r.IsAvailable && r.PricePerNight <= searchDto.MaxPricePerNight.Value));

            return hotels.Select(MapToResponseDto);
        }

        public async Task<HotelResponseDto> CreateHotelAsync(HotelDto hotelDto)
        {
            var hotel = new Hotel
            {
                Name = hotelDto.Name,
                Description = hotelDto.Description,
                Location = hotelDto.Location,
                City = hotelDto.City,
                Country = hotelDto.Country,
                StarRating = hotelDto.StarRating,
                PhoneNumber = hotelDto.PhoneNumber,
                Email = hotelDto.Email,
                Amenities = hotelDto.Amenities,
                ImageUrl = hotelDto.ImageUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _hotelRepository.AddAsync(hotel);
            return MapToResponseDto(created);
        }

        public async Task<HotelResponseDto?> UpdateHotelAsync(int id, HotelDto hotelDto)
        {
            var hotel = await _hotelRepository.GetByIdAsync(id);
            if (hotel == null) return null;

            // Update fields
            hotel.Name = hotelDto.Name;
            hotel.Description = hotelDto.Description;
            hotel.Location = hotelDto.Location;
            hotel.City = hotelDto.City;
            hotel.Country = hotelDto.Country;
            hotel.StarRating = hotelDto.StarRating;
            hotel.PhoneNumber = hotelDto.PhoneNumber;
            hotel.Email = hotelDto.Email;
            hotel.Amenities = hotelDto.Amenities;
            hotel.ImageUrl = hotelDto.ImageUrl;

            var updated = await _hotelRepository.UpdateAsync(hotel);
            return MapToResponseDto(updated);
        }

        public async Task<bool> DeleteHotelAsync(int id)
        {
            return await _hotelRepository.DeleteAsync(id);
        }

        // Helper: convert Hotel model → HotelResponseDto
        private static HotelResponseDto MapToResponseDto(Hotel hotel)
        {
            var activeRooms = hotel.Rooms.Where(r => r.IsActive).ToList();
            return new HotelResponseDto
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Description = hotel.Description,
                Location = hotel.Location,
                City = hotel.City,
                Country = hotel.Country,
                StarRating = hotel.StarRating,
                PhoneNumber = hotel.PhoneNumber,
                Email = hotel.Email,
                Amenities = hotel.Amenities,
                ImageUrl = hotel.ImageUrl,
                TotalRooms = activeRooms.Count,
                AvailableRooms = activeRooms.Count(r => r.IsAvailable),
                LowestPrice = activeRooms.Any() ? activeRooms.Min(r => r.PricePerNight) : null
            };
        }
    }
}