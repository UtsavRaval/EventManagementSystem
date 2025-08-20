using EventManagement.api.DTOs;
using EventManagement.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("admin-login")]
        public async Task<IActionResult> AdminLogin(AdminLoginDto loginDto)
        {
            var response = await _authService.AdminLoginAsync(loginDto);

            if (response == null)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(response);
        }

        [HttpPost("user-login")]
        public async Task<IActionResult> UserLogin(UserLoginDto loginDto)
        {
            var response = await _authService.UserLoginAsync(loginDto);

            if (response == null)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(response);
        }

        [HttpPost("user-register")]
        public async Task<IActionResult> UserRegister(UserRegistrationDto registrationDto)
        {
            var response = await _authService.UserRegisterAsync(registrationDto);

            if (response == null)
                return BadRequest(new { message = "Email or enrollment number already exists" });

            return Ok(response);
        }
    }
}
