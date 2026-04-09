namespace HotelBooking.Dto.Room
{
    public class CreateRoomDto
    {
        public int HotelId { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }
}
