namespace HotelBooking.Dto.Booking
{
    public class BookingHistoryDto
    {
        public int BookingId { get; set; }
        public string HotelName { get; set; }
        public string RoomCategory { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public decimal Price { get; set; }
        public string Status { get; set; }
    }
}
