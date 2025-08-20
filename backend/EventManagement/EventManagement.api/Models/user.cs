using Microsoft.Extensions.Logging;

namespace EventManagement.api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int? Sem { get; set; }
        public string? Branch { get; set; }
        public string? EnrollmentNumber { get; set; }
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Event> CreatedEvents { get; set; } = new List<Event>();
        public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
    }
}

