using HotelBooking.Dto.Room;

namespace HotelBooking.Dto.Hotel
{
    public class HotelResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public List<RoomResponseDto> Rooms { get; set; }
    }
}
