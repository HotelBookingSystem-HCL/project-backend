// Interfaces/IBookingService.cs

using HotelBooking.DTOs;

namespace HotelBooking.Interfaces
{
    public interface IBookingService
    {
        // NOTE: Takes BookingDto (the request), NOT BookingResponseDto
        Task<BookingResponseDto> CreateBookingAsync(int userId, BookingDto bookingDto);
        Task<BookingResponseDto?> GetBookingByIdAsync(int id);
        Task<IEnumerable<BookingResponseDto>> GetUserBookingsAsync(int userId);
        Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync();
        Task<BookingResponseDto?> UpdateBookingStatusAsync(int id, string status);
        Task<bool> CancelBookingAsync(int bookingId, int userId, bool isAdmin);
    }
}