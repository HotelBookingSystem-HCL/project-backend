using HotelBooking.Dto.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers;

[Authorize]
[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _service;

    public BookingsController(IBookingService service)
    {
        _service = service;
    }

    // Book a room
    [HttpPost]
    public async Task<IActionResult> Book(CreateBookingDto dto)
    {
        var userId = int.Parse(User.FindFirst("UserId")!.Value);

        var result = await _service.Book(userId, dto);

        return Ok(result);
    }

    // Get user bookings
    [HttpGet("my")]
    public async Task<IActionResult> MyBookings()
    {
        var userId = int.Parse(User.FindFirst("UserId")!.Value);

        var bookings = await _service.GetUserBookings(userId);

        return Ok(bookings);
    }
}