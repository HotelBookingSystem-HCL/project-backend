// Controllers/HotelsController.cs
// Public: GET endpoints (anyone can browse hotels)
// Admin only: POST, PUT, DELETE (only admins can manage hotels)

using HotelBooking.DTOs;
using HotelBooking.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route = api/hotels
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        // GET api/hotels
        // Public - anyone can see all hotels
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var hotels = await _hotelService.GetAllHotelsAsync();
            return Ok(hotels);
        }

        // GET api/hotels/5
        // Public - anyone can view a specific hotel
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null)
                return NotFound(new { Message = $"Hotel with ID {id} not found." });

            return Ok(hotel);
        }

        // GET api/hotels/search?city=Mumbai&minStarRating=3&maxPricePerNight=5000
        // Public - search with filters
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] HotelSearchDto searchDto)
        {
            var results = await _hotelService.SearchHotelsAsync(searchDto);
            return Ok(results);
        }

        // POST api/hotels
        // Admin only - create a new hotel
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] HotelDto hotelDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _hotelService.CreateHotelAsync(hotelDto);

            // Return 201 Created with the location header pointing to the new resource
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/hotels/5
        // Admin only - update a hotel
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] HotelDto hotelDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _hotelService.UpdateHotelAsync(id, hotelDto);
            if (updated == null)
                return NotFound(new { Message = $"Hotel with ID {id} not found." });

            return Ok(updated);
        }

        // DELETE api/hotels/5
        // Admin only - soft-delete a hotel
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _hotelService.DeleteHotelAsync(id);
            if (!deleted)
                return NotFound(new { Message = $"Hotel with ID {id} not found." });

            return Ok(new { Message = "Hotel deleted successfully." });
        }
    }
}