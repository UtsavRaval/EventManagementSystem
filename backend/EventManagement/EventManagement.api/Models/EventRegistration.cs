namespace EventManagement.api.Models
{
    public class EventRegistration
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Event Event { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
