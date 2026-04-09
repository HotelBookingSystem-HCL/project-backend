namespace HotelBooking.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;

        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }
}
