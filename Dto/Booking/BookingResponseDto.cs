namespace HotelBooking.Dto.Booking
{
    public class BookingResponseDto
    {

        public int BookingId { get; set; }
        public int RoomId { get; set; }
        public string HotelName { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public string Status { get; set; }
    }
}
