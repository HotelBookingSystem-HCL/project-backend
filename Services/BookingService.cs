// Services/BookingService.cs

using HotelBooking.DTOs;
using HotelBooking.Interfaces;
using HotelBooking.Models;

namespace HotelBooking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;

        public BookingService(IBookingRepository bookingRepository, IRoomRepository roomRepository)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
        }

        public async Task<BookingResponseDto> CreateBookingAsync(int userId, BookingDto bookingDto)
        {
            if (bookingDto.CheckInDate >= bookingDto.CheckOutDate)
                throw new ArgumentException("Check-out date must be after check-in date.");

            if (bookingDto.CheckInDate < DateTime.UtcNow.Date)
                throw new ArgumentException("Check-in date cannot be in the past.");

            var room = await _roomRepository.GetByIdAsync(bookingDto.RoomId)
                ?? throw new KeyNotFoundException("Room not found.");

            if (!room.IsAvailable || !room.IsActive)
                throw new InvalidOperationException("This room is not available for booking.");

            var isAvailable = await _roomRepository.IsRoomAvailableAsync(
                bookingDto.RoomId, bookingDto.CheckInDate, bookingDto.CheckOutDate);

            if (!isAvailable)
                throw new InvalidOperationException("Room is already booked for the selected dates.");

            if (bookingDto.NumberOfGuests > room.MaxOccupancy)
                throw new ArgumentException($"Room max occupancy is {room.MaxOccupancy} guests.");

            int nights = (bookingDto.CheckOutDate - bookingDto.CheckInDate).Days;
            decimal totalAmount = nights * room.PricePerNight;

            // Apply promo codes
            decimal discountAmount = 0;
            if (!string.IsNullOrWhiteSpace(bookingDto.PromoCode))
            {
                discountAmount = bookingDto.PromoCode.ToUpper() switch
                {
                    "SAVE10" => totalAmount * 0.10m,
                    "SUMMER20" => totalAmount * 0.20m,
                    _ => 0
                };
            }

            totalAmount -= discountAmount;

            var booking = new Booking
            {
                ReservationNumber = await _bookingRepository.GenerateReservationNumberAsync(),
                UserId = userId,
                RoomId = bookingDto.RoomId,
                CheckInDate = bookingDto.CheckInDate,
                CheckOutDate = bookingDto.CheckOutDate,
                NumberOfGuests = bookingDto.NumberOfGuests,
                TotalAmount = totalAmount,
                DiscountAmount = discountAmount,
                Status = "Confirmed",
                SpecialRequests = bookingDto.SpecialRequests,
                PromoCode = bookingDto.PromoCode,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _bookingRepository.AddAsync(booking);
            var full = await _bookingRepository.GetByIdAsync(created.Id);
            return MapToDto(full!);
        }

        public async Task<BookingResponseDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            return booking == null ? null : MapToDto(booking);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetUserBookingsAsync(int userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            return bookings.Select(MapToDto);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return bookings.Select(MapToDto);
        }

        public async Task<BookingResponseDto?> UpdateBookingStatusAsync(int id, string status)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return null;

            var valid = new[] { "Pending", "Confirmed", "Cancelled", "Completed" };
            if (!valid.Contains(status))
                throw new ArgumentException($"Invalid status. Use: {string.Join(", ", valid)}");

            booking.Status = status;
            await _bookingRepository.UpdateAsync(booking);

            var updated = await _bookingRepository.GetByIdAsync(id);
            return MapToDto(updated!);
        }

        public async Task<bool> CancelBookingAsync(int bookingId, int userId, bool isAdmin)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return false;

            if (!isAdmin && booking.UserId != userId)
                throw new UnauthorizedAccessException("You can only cancel your own bookings.");

            if (booking.Status == "Cancelled")
                throw new InvalidOperationException("Booking is already cancelled.");

            if (booking.Status == "Completed")
                throw new InvalidOperationException("Completed bookings cannot be cancelled.");

            booking.Status = "Cancelled";
            await _bookingRepository.UpdateAsync(booking);
            return true;
        }

        private static BookingResponseDto MapToDto(Booking b) => new()
        {
            Id = b.Id,
            ReservationNumber = b.ReservationNumber,
            HotelName = b.Room?.Hotel?.Name ?? "",
            RoomType = b.Room?.RoomType ?? "",
            RoomNumber = b.Room?.RoomNumber ?? "",
            CheckInDate = b.CheckInDate,
            CheckOutDate = b.CheckOutDate,
            NumberOfGuests = b.NumberOfGuests,
            TotalAmount = b.TotalAmount,
            DiscountAmount = b.DiscountAmount,
            Status = b.Status,
            SpecialRequests = b.SpecialRequests,
            CreatedAt = b.CreatedAt
        };
    }
}