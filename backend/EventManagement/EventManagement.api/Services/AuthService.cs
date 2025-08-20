using EventManagement.api.Data;
using EventManagement.api.DTOs;
using EventManagement.api.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using BC = BCrypt.Net.BCrypt;


namespace EventManagement.api.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> AdminLoginAsync(AdminLoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.Role == "Admin");

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                return null;

            var token = GenerateJwtToken(user);
            return new AuthResponseDto
            {
                Token = token,
                Role = user.Role,
                Name = user.Name,
                Email = user.Email
            };
        }

        public async Task<AuthResponseDto?> UserLoginAsync(UserLoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.Role == "User");

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                return null;

            var token = GenerateJwtToken(user);
            return new AuthResponseDto
            {
                Token = token,
                Role = user.Role,
                Name = user.Name,
                Email = user.Email
            };
        }

        public async Task<AuthResponseDto?> UserRegisterAsync(UserRegistrationDto registrationDto)
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == registrationDto.Email))
                return null;

            // Check if enrollment number already exists
            if (await _context.Users.AnyAsync(u => u.EnrollmentNumber == registrationDto.EnrollmentNumber))
                return null;

            var user = new User
            {
                Email = registrationDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password),
                Name = registrationDto.Name,
                Sem = registrationDto.Sem,
                Branch = registrationDto.Branch,
                EnrollmentNumber = registrationDto.EnrollmentNumber,
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return new AuthResponseDto
            {
                Token = token,
                Role = user.Role,
                Name = user.Name,
                Email = user.Email
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSection["Key"] ?? throw new InvalidOperationException("JWT Key not found"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("email", user.Email),
                    new Claim("role", user.Role),
                    new Claim("name", user.Name)
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = jwtSection["Issuer"],
                Audience = jwtSection["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
