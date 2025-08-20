namespace EventManagement.api.Models
{
    public class Event
    {
        public int Id { get; set; }
        public int SrNo { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; }

        // Navigation properties
        public User Creator { get; set; } = null!;
        public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
    }
}
