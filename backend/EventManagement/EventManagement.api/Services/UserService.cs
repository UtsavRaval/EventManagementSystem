using EventManagement.api.Data;
using EventManagement.api.DTOs;
using Microsoft.EntityFrameworkCore;
namespace EventManagement.api.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Where(u => u.Role == "User")
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Name = u.Name,
                    Sem = u.Sem,
                    Branch = u.Branch,
                    EnrollmentNumber = u.EnrollmentNumber,
                    CreatedAt = u.CreatedAt
                })
                .OrderBy(u => u.Name)
                .ToListAsync();

            return users;
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Name = u.Name,
                    Sem = u.Sem,
                    Branch = u.Branch,
                    EnrollmentNumber = u.EnrollmentNumber,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync();

            return user;
        }
    }
}
