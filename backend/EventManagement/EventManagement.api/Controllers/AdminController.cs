using EventManagement.api.DTOs;
using EventManagement.api.Models;
using EventManagement.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.api.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        [Authorize(Roles = "Admin")]
        public class AdminController : ControllerBase
        {
            private readonly IEventService _eventService;
            private readonly IUserService _userService;

            public AdminController(IEventService eventService, IUserService userService)
            {
                _eventService = eventService;
                _userService = userService;
            }

            private int GetCurrentUserId()
            {
                var userIdClaim = User.FindFirst("id")?.Value;
                return int.Parse(userIdClaim ?? "0");
            }

            [HttpGet("dashboard-stats")]
            public async Task<IActionResult> GetDashboardStats()
            {
                var stats = await _eventService.GetDashboardStatsAsync();
                return Ok(stats);
            }

            [HttpGet("users")]
            public async Task<IActionResult> GetAllUsers()
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }

            [HttpGet("events")]
            public async Task<IActionResult> GetAllEvents()
            {
                var events = await _eventService.GetAllEventsAsync();
                return Ok(events);
            }

            [HttpGet("events/{id}/details")]
            public async Task<IActionResult> GetEventDetails(int id)
            {
                var eventDetails = await _eventService.GetEventDetailsAsync(id);

                if (eventDetails == null)
                    return NotFound(new { message = "Event not found" });

                return Ok(eventDetails);
            }

            [HttpPost("events")]
            public async Task<IActionResult> CreateEvent(CreateEventDto createEventDto)
            {
                var createdBy = GetCurrentUserId();
                var eventDto = await _eventService.CreateEventAsync(createEventDto, createdBy);

                if (eventDto == null)
                    return BadRequest(new { message = "Event creation failed. Serial number might already exist or there's a time conflict." });

                return CreatedAtAction(nameof(GetEventDetails), new { id = eventDto.Id }, eventDto);
            }

            [HttpPut("events/{id}")]
            public async Task<IActionResult> UpdateEvent(int id, UpdateEventDto updateEventDto)
            {
                var eventDto = await _eventService.UpdateEventAsync(id, updateEventDto);

                if (eventDto == null)
                    return BadRequest(new { message = "Event update failed. Check for conflicts or invalid data." });

                return Ok(eventDto);
            }

            [HttpDelete("events/{id}")]
            public async Task<IActionResult> DeleteEvent(int id)
            {
                var success = await _eventService.DeleteEventAsync(id);

                if (!success)
                    return NotFound(new { message = "Event not found" });

                return Ok(new { message = "Event deleted successfully" });
            }
        }
}
