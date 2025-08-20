using EventManagement.api.Data;
using EventManagement.api.DTOs;
using EventManagement.api.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.api.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EventDto>> GetAllEventsAsync()
        {
            var events = await _context.Events
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    SrNo = e.SrNo,
                    EventName = e.EventName,
                    EventDate = e.EventDate,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    TotalSeats = e.TotalSeats,
                    AvailableSeats = e.AvailableSeats,
                    SeatsFull = e.TotalSeats - e.AvailableSeats,
                    CreatedAt = e.CreatedAt
                })
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            return events;
        }

        public async Task<List<EventDto>> GetAvailableEventsAsync(int userId)
        {
            var events = await _context.Events
                .Where(e => e.EventDate >= DateTime.UtcNow)
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    SrNo = e.SrNo,
                    EventName = e.EventName,
                    EventDate = e.EventDate,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    TotalSeats = e.TotalSeats,
                    AvailableSeats = e.AvailableSeats,
                    SeatsFull = e.TotalSeats - e.AvailableSeats,
                    IsRegistered = e.EventRegistrations.Any(er => er.UserId == userId),
                    CanRegister = e.AvailableSeats > 0 && !e.EventRegistrations.Any(er => er.UserId == userId),
                    CreatedAt = e.CreatedAt
                })
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            return events;
        }

        public async Task<EventDetailsDto?> GetEventDetailsAsync(int eventId)
        {
            var eventDetails = await _context.Events
                .Where(e => e.Id == eventId)
                .Select(e => new EventDetailsDto
                {
                    Id = e.Id,
                    SrNo = e.SrNo,
                    EventName = e.EventName,
                    EventDate = e.EventDate,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    TotalSeats = e.TotalSeats,
                    AvailableSeats = e.AvailableSeats,
                    SeatsFull = e.TotalSeats - e.AvailableSeats,
                    RegisteredUsers = e.EventRegistrations.Select(er => new RegisteredUserDto
                    {
                        Name = er.User.Name,
                        Email = er.User.Email,
                        EnrollmentNumber = er.User.EnrollmentNumber ?? "",
                        Branch = er.User.Branch ?? "",
                        Sem = er.User.Sem ?? 0,
                        RegisteredAt = er.RegisteredAt
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return eventDetails;
        }

        public async Task<EventDto?> CreateEventAsync(CreateEventDto createEventDto, int createdBy)
        {
            // Check if SrNo already exists
            if (await _context.Events.AnyAsync(e => e.SrNo == createEventDto.SrNo))
                return null;

            // Check for overlapping events on the same date
            if (await HasTimeConflictAsync(createEventDto.EventDate, createEventDto.StartTime, createEventDto.EndTime))
                return null;

            // Prevent past-dated events
            if (createEventDto.EventDate < DateTime.Today)
                return null;

            // Validate time range
            if (createEventDto.StartTime >= createEventDto.EndTime)
                return null;

            var eventEntity = new Event
            {
                SrNo = createEventDto.SrNo,
                EventName = createEventDto.EventName,
                EventDate = createEventDto.EventDate,
                StartTime = createEventDto.StartTime,
                EndTime = createEventDto.EndTime,
                TotalSeats = createEventDto.TotalSeats,
                AvailableSeats = createEventDto.TotalSeats,
                CreatedBy = createdBy
            };

            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();

            return new EventDto
            {
                Id = eventEntity.Id,
                SrNo = eventEntity.SrNo,
                EventName = eventEntity.EventName,
                EventDate = eventEntity.EventDate,
                StartTime = eventEntity.StartTime,
                EndTime = eventEntity.EndTime,
                TotalSeats = eventEntity.TotalSeats,
                AvailableSeats = eventEntity.AvailableSeats,
                SeatsFull = 0,
                CreatedAt = eventEntity.CreatedAt
            };
        }

        public async Task<EventDto?> UpdateEventAsync(int eventId, UpdateEventDto updateEventDto)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null) return null;

            // Check if SrNo conflicts with other events
            if (await _context.Events.AnyAsync(e => e.SrNo == updateEventDto.SrNo && e.Id != eventId))
                return null;

            // Check for time conflicts
            if (await HasTimeConflictAsync(updateEventDto.EventDate, updateEventDto.StartTime, updateEventDto.EndTime, eventId))
                return null;

            var registeredSeats = eventEntity.TotalSeats - eventEntity.AvailableSeats;

            // Can't reduce seats below registered count
            if (updateEventDto.TotalSeats < registeredSeats)
                return null;

            eventEntity.SrNo = updateEventDto.SrNo;
            eventEntity.EventName = updateEventDto.EventName;
            eventEntity.EventDate = updateEventDto.EventDate;
            eventEntity.StartTime = updateEventDto.StartTime;
            eventEntity.EndTime = updateEventDto.EndTime;
            eventEntity.TotalSeats = updateEventDto.TotalSeats;
            eventEntity.AvailableSeats = updateEventDto.TotalSeats - registeredSeats;

            await _context.SaveChangesAsync();

            return new EventDto
            {
                Id = eventEntity.Id,
                SrNo = eventEntity.SrNo,
                EventName = eventEntity.EventName,
                EventDate = eventEntity.EventDate,
                StartTime = eventEntity.StartTime,
                EndTime = eventEntity.EndTime,
                TotalSeats = eventEntity.TotalSeats,
                AvailableSeats = eventEntity.AvailableSeats,
                SeatsFull = eventEntity.TotalSeats - eventEntity.AvailableSeats,
                CreatedAt = eventEntity.CreatedAt
            };
        }

        public async Task<bool> DeleteEventAsync(int eventId)
        {
            var eventEntity = await _context.Events
                .Include(e => e.EventRegistrations)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventEntity == null) return false;

            _context.EventRegistrations.RemoveRange(eventEntity.EventRegistrations);
            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RegisterForEventAsync(int eventId, int userId)
        {
            // Check if already registered
            if (await _context.EventRegistrations.AnyAsync(er => er.EventId == eventId && er.UserId == userId))
                return false;

            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null || eventEntity.AvailableSeats <= 0)
                return false;

            var registration = new EventRegistration
            {
                EventId = eventId,
                UserId = userId
            };

            _context.EventRegistrations.Add(registration);
            eventEntity.AvailableSeats--;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<EventDto>> GetUserRegistrationsAsync(int userId)
        {
            var registrations = await _context.EventRegistrations
                .Where(er => er.UserId == userId)
                .Select(er => new EventDto
                {
                    Id = er.Event.Id,
                    SrNo = er.Event.SrNo,
                    EventName = er.Event.EventName,
                    EventDate = er.Event.EventDate,
                    StartTime = er.Event.StartTime,
                    EndTime = er.Event.EndTime,
                    TotalSeats = er.Event.TotalSeats,
                    AvailableSeats = er.Event.AvailableSeats,
                    SeatsFull = er.Event.TotalSeats - er.Event.AvailableSeats,
                    IsRegistered = true,
                    CreatedAt = er.Event.CreatedAt
                })
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            return registrations;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var totalUsers = await _context.Users.CountAsync(u => u.Role == "User");
            var totalEvents = await _context.Events.CountAsync();

            return new DashboardStatsDto
            {
                TotalUsers = totalUsers,
                TotalEvents = totalEvents
            };
        }

        public async Task<bool> HasTimeConflictAsync(DateTime eventDate, TimeSpan startTime, TimeSpan endTime, int? excludeEventId = null)
        {
            return await _context.Events
                .Where(e => e.EventDate.Date == eventDate.Date &&
                           (excludeEventId == null || e.Id != excludeEventId))
                .AnyAsync(e => startTime < e.EndTime && endTime > e.StartTime);
        }
    }

}
