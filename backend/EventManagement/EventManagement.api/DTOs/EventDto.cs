namespace EventManagement.api.DTOs
{

    public class CreateEventDto
    {
        public int SrNo { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int TotalSeats { get; set; }
    }

    public class UpdateEventDto
    {
        public int SrNo { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int TotalSeats { get; set; }
    }

    public class EventDto
    {
        public int Id { get; set; }
        public int SrNo { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public int SeatsFull { get; set; }
        public bool CanRegister { get; set; }
        public bool IsRegistered { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EventDetailsDto
    {
        public int Id { get; set; }
        public int SrNo { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public int SeatsFull { get; set; }
        public List<RegisteredUserDto> RegisteredUsers { get; set; } = new List<RegisteredUserDto>();
    }

    public class RegisteredUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EnrollmentNumber { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public int Sem { get; set; }
        public DateTime RegisteredAt { get; set; }
    }

    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalEvents { get; set; }
    }
}
