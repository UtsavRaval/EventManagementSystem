using EventManagement.api.DTOs;

namespace EventManagement.api.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int userId);
    }
}
