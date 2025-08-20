namespace EventManagement.api.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int? Sem { get; set; }
        public string? Branch { get; set; }
        public string? EnrollmentNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
