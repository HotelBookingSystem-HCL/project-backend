// Controllers/UsersController.cs
// Admin-only: manage all users (view, update, deactivate)
// Users: view/update their own profile

using HotelBooking.DTOs;
using HotelBooking.Interfaces;
using HotelBooking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route = api/users
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(claim ?? "0");
        }

        // GET api/users
        // Admin only - list all users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            var dtos = users.Select(u => new UserProfileDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt,
                IsActive = u.IsActive
            });
            return Ok(dtos);
        }

        // GET api/users/5
        // Admin can see any user; users can only see themselves
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Regular users can only view their own profile
            if (!User.IsInRole("Admin") && GetCurrentUserId() != id)
                return Forbid();

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { Message = $"User {id} not found." });

            return Ok(new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            });
        }

        // GET api/users/me
        // Shortcut: any user can get their own profile using this endpoint
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            return await GetById(GetCurrentUserId());
        }

        // PUT api/users/5
        // Admin: can update any user including role and active status
        // User: can update only their own name and email
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto updateDto)
        {
            var isAdmin = User.IsInRole("Admin");

            // Regular users can only edit themselves
            if (!isAdmin && GetCurrentUserId() != id)
                return Forbid();

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { Message = $"User {id} not found." });

            // Everyone can update name and email
            user.FullName = updateDto.FullName;
            user.Email = updateDto.Email;

            // Only admin can change role and active status
            if (isAdmin)
            {
                user.Role = updateDto.Role;
                user.IsActive = updateDto.IsActive;
            }

            await _userRepository.UpdateAsync(user);

            return Ok(new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            });
        }

        // DELETE api/users/5
        // Admin only - deactivate (soft-delete) a user
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            // Prevent admin from deleting themselves
            if (GetCurrentUserId() == id)
                return BadRequest(new { Message = "You cannot delete your own admin account." });

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { Message = $"User {id} not found." });

            // Soft delete - just deactivate
            user.IsActive = false;
            await _userRepository.UpdateAsync(user);

            return Ok(new { Message = "User deactivated successfully." });
        }
    }
}