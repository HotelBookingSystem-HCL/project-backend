using HotelBooking.DTOs;

namespace HotelBooking.Interfaces
{
    public interface IEmailService
    {
        Task SendBookingConfirmationAsync(string toEmail, string toName, BookingResponseDto booking);
    }
}