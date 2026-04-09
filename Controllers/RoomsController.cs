using HotelBooking.Dto.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _service;

    public RoomsController(IRoomService service)
    {
        _service = service;
    }

    // Get all rooms
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rooms = await _service.GetAll();

        var result = rooms.Select(r => new RoomResponseDto
        {
            Id = r.Id,
            Category = r.Category,
            Price = r.Price,
            IsAvailable = r.IsAvailable,
            HotelName = r.Hotel?.Name,
            Location = r.Hotel?.Location
        });

        return Ok(result);
    }

    // Admin: add room
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Add(CreateRoomDto dto)
    {
        var room = new Models.Room
        {
            HotelId = dto.HotelId,
            Category = dto.Category,
            Price = dto.Price
        };

        var result = await _service.Add(room);

        return Ok(result);
    }

    // Search & filter
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] RoomSearchDto dto)
    {
        var rooms = await _service.Search(dto.Location, dto.MinPrice, dto.MaxPrice);

        var result = rooms.Select(r => new RoomResponseDto
        {
            Id = r.Id,
            Category = r.Category,
            Price = r.Price,
            IsAvailable = r.IsAvailable,
            HotelName = r.Hotel?.Name,
            Location = r.Hotel?.Location
        });

        return Ok(result);
    }
}