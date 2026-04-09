// Controllers/RoomsController.cs
// Rooms are nested under hotels: api/hotels/{hotelId}/rooms
// Public: browse and search rooms
// Admin only: create, update, delete

using HotelBooking.DTOs;
using HotelBooking.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("api/hotels/{hotelId}/rooms")] // Nested route under hotels
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // GET api/hotels/3/rooms
        // Public - get all rooms for a hotel
        [HttpGet]
        public async Task<IActionResult> GetByHotel(int hotelId)
        {
            var rooms = await _roomService.GetRoomsByHotelAsync(hotelId);
            return Ok(rooms);
        }

        // GET api/hotels/3/rooms/7
        // Public - get a specific room
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int hotelId, int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null || room.HotelId != hotelId)
                return NotFound(new { Message = $"Room with ID {id} not found in hotel {hotelId}." });

            return Ok(room);
        }

        // GET api/hotels/3/rooms/search?checkInDate=2024-05-01&checkOutDate=2024-05-05&numberOfGuests=2
        // Public - search available rooms for given dates
        [HttpGet("search")]
        public async Task<IActionResult> SearchAvailable(int hotelId, [FromQuery] RoomSearchDto searchDto)
        {
            searchDto.HotelId = hotelId; // Ensure the hotelId from route is used
            var results = await _roomService.SearchAvailableRoomsAsync(searchDto);
            return Ok(results);
        }

        // POST api/hotels/3/rooms
        // Admin only - add a room to a hotel
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(int hotelId, [FromBody] RoomDto roomDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _roomService.CreateRoomAsync(hotelId, roomDto);
            return CreatedAtAction(nameof(GetById), new { hotelId, id = created.Id }, created);
        }

        // PUT api/hotels/3/rooms/7
        // Admin only - update room details
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int hotelId, int id, [FromBody] RoomDto roomDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _roomService.UpdateRoomAsync(id, roomDto);
            if (updated == null)
                return NotFound(new { Message = $"Room with ID {id} not found." });

            return Ok(updated);
        }

        // DELETE api/hotels/3/rooms/7
        // Admin only - soft-delete a room
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int hotelId, int id)
        {
            var deleted = await _roomService.DeleteRoomAsync(id);
            if (!deleted)
                return NotFound(new { Message = $"Room with ID {id} not found." });

            return Ok(new { Message = "Room deleted successfully." });
        }
    }
}