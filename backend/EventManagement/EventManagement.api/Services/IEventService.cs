using EventManagement.api.DTOs;

namespace EventManagement.api.Services
{
    public interface IEventService
    {
        Task<List<EventDto>> GetAllEventsAsync();
        Task<List<EventDto>> GetAvailableEventsAsync(int userId);
        Task<EventDetailsDto?> GetEventDetailsAsync(int eventId);
        Task<EventDto?> CreateEventAsync(CreateEventDto createEventDto, int createdBy);
        Task<EventDto?> UpdateEventAsync(int eventId, UpdateEventDto updateEventDto);
        Task<bool> DeleteEventAsync(int eventId);
        Task<bool> RegisterForEventAsync(int eventId, int userId);
        Task<List<EventDto>> GetUserRegistrationsAsync(int userId);
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        Task<bool> HasTimeConflictAsync(DateTime eventDate, TimeSpan startTime, TimeSpan endTime, int? excludeEventId = null);
    }
}
