using HotelBooking.Dto.Booking;

namespace HotelBooking.Interfaces
{
    public interface IBookingService
    {
        Task<string> Book(int userId, CreateBookingDto dto);
        Task<List<BookingHistoryDto>> GetUserBookings(int userId);
    }
}
