using EventManagement.api.Models;
using EventManagement.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class UserController : ControllerBase
    {
        private readonly IEventService _eventService;

        public UserController(IEventService eventService)
        {
            _eventService = eventService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        [HttpGet("events")]
        public async Task<IActionResult> GetAvailableEvents()
        {
            var userId = GetCurrentUserId();
            var events = await _eventService.GetAvailableEventsAsync(userId);
            return Ok(events);
        }

        [HttpPost("events/{id}/register")]
        public async Task<IActionResult> RegisterForEvent(int id)
        {
            var userId = GetCurrentUserId();
            var success = await _eventService.RegisterForEventAsync(id, userId);

            if (!success)
                return BadRequest(new { message = "Registration failed. Event might be full or you're already registered." });

            return Ok(new { message = "Successfully registered for the event" });
        }

        [HttpGet("my-registrations")]
        public async Task<IActionResult> GetMyRegistrations()
        {
            var userId = GetCurrentUserId();
            var registrations = await _eventService.GetUserRegistrationsAsync(userId);
            return Ok(registrations);
        }
    }
}
