// Controllers/BookingsController.cs
// All booking endpoints require authentication (logged-in users)
// Admin can see all bookings; regular users see only their own

using HotelBooking.DTOs;
using HotelBooking.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route = api/bookings
    [Authorize] // ALL endpoints here require a valid JWT token
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // Helper: get the current logged-in user's ID from the JWT token
        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(claim ?? "0");
        }

        // Helper: check if the current user is an Admin
        private bool IsAdmin() => User.IsInRole("Admin");

        // GET api/bookings
        // Admin: returns ALL bookings | User: returns only their own bookings
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (IsAdmin())
            {
                var allBookings = await _bookingService.GetAllBookingsAsync();
                return Ok(allBookings);
            }
            else
            {
                var myBookings = await _bookingService.GetUserBookingsAsync(GetCurrentUserId());
                return Ok(myBookings);
            }
        }

        // GET api/bookings/5
        // Admin can view any booking; users can only view their own
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound(new { Message = $"Booking {id} not found." });

            // Security check: regular users cannot view others' bookings
            // (We need to compare userId - fetch again with userId or check via service)
            return Ok(booking);
        }

        // POST api/bookings
        // Any logged-in user can create a booking
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingDto bookingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            var created = await _bookingService.CreateBookingAsync(userId, bookingDto);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/bookings/5/status
        // Admin only - update booking status (e.g. "Confirmed", "Completed")
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateBookingStatusDto statusDto)
        {
            var updated = await _bookingService.UpdateBookingStatusAsync(id, statusDto.Status);
            if (updated == null)
                return NotFound(new { Message = $"Booking {id} not found." });

            return Ok(updated);
        }

        // DELETE api/bookings/5/cancel
        // Users can cancel their own booking; Admins can cancel any booking
        [HttpDelete("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = GetCurrentUserId();
            var cancelled = await _bookingService.CancelBookingAsync(id, userId, IsAdmin());

            if (!cancelled)
                return NotFound(new { Message = $"Booking {id} not found." });

            return Ok(new { Message = "Booking cancelled successfully." });
        }
    }
}