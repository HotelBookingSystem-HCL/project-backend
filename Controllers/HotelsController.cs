using HotelBooking.Dto.Hotel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace HotelBooking.Controllers;

[ApiController]
[Route("api/hotels")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _service;

    public HotelsController(IHotelService service)
    {
        _service = service;
    }

    // Get all hotels
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var hotels = await _service.GetAll();

        var result = hotels.Select(h => new HotelResponseDto
        {
            Id = h.Id,
            Name = h.Name,
            Location = h.Location,
            Rooms = h.Rooms?.Select(r => new DTOs.Room.RoomResponseDto
            {
                Id = r.Id,
                Category = r.Category,
                Price = r.Price,
                IsAvailable = r.IsAvailable,
                HotelName = h.Name,
                Location = h.Location
            }).ToList()
        });

        return Ok(result);
    }

    // Admin: create hotel
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateHotelDto dto)
    {
        var hotel = new Models.Hotel
        {
            Name = dto.Name,
            Location = dto.Location
        };

        var result = await _service.Create(hotel);

        return Ok(new HotelResponseDto
        {
            Id = result.Id,
            Name = result.Name,
            Location = result.Location
        });
    }
}