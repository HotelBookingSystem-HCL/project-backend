namespace HotelBooking.Dto.Room
{
    public class RoomResponseDto
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }

        public string HotelName { get; set; }
        public string Location { get; set; }
    }
}
