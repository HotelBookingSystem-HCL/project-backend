namespace HotelBooking.Dto.Booking
{
    public class CreateBookingDto
    {
        public int RoomId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
