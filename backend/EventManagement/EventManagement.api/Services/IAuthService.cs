using EventManagement.api.DTOs;

namespace EventManagement.api.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> AdminLoginAsync(AdminLoginDto loginDto);
        Task<AuthResponseDto?> UserLoginAsync(UserLoginDto loginDto);
        Task<AuthResponseDto?> UserRegisterAsync(UserRegistrationDto registrationDto);
    }
}
