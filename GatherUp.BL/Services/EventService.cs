using GatherUp.core.DO;
using GatherUp.core.Exceptions;
using GatherUp.core.interfaces;
using GatherUp.Infrastructure.Services;

namespace GatherUp.BL.Services
{
    public class EventService : IEventService
    {
        private readonly IRepository<Event> _eventRepo;
        private readonly IRepository<Participant> _participantRepo;

        public EventService(IRepository<Event> eventRepo, IRepository<Participant> participantRepo)
        {
            _eventRepo = eventRepo;
            _participantRepo = participantRepo;
        }

        public event Action<int>? OnEventDetailsChanged;

        public Event GetEventById(int eventId) =>
            _eventRepo.GetById(eventId) ?? throw new EntityNotFoundException("Event", eventId);

        public bool IsManager(int eventId, int userId) =>
            _eventRepo.GetById(eventId)?.EventManagerId == userId;

        public Event CreateEvent(string title, DateTime date, string location, int managerId, int hostId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new InvalidEventDataException("Event title cannot be empty.");

            var creator = _participantRepo.GetById(managerId);
            if (creator != null && creator.Role == UserRole.Participant)
            {
                creator.Role = UserRole.Manager;
                _participantRepo.Update(creator);
            }

            var newId = _eventRepo.GetAll().Any() ? _eventRepo.GetAll().Max(e => e.Id) + 1 : 1;
            var ev = new Event
            {
                Id = newId,
                Title = title,
                Date = date,
                Location = location,
                EventManagerId = managerId,
                EventHostId = hostId,
                ParticipantIds = new List<int> { managerId },
                ParticipantData = new List<EventParticipantData>
                {
                    new EventParticipantData { ParticipantId = managerId, IsAttending = true }
                }
            };
            _eventRepo.Add(ev);
            return ev;
        }

        public void UpdateEvent(int eventId, string title, DateTime date, string location)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new EntityNotFoundException("Event", eventId);
            if (string.IsNullOrWhiteSpace(title))
                throw new InvalidEventDataException("Event title cannot be empty.");

            ev.Title = title;
            ev.Date = date;
            ev.Location = location;
            _eventRepo.Update(ev);
            OnEventDetailsChanged?.Invoke(eventId);
        }

        public IEnumerable<Event> GetEventsByManager(int managerId) =>
            _eventRepo.GetAll().Where(e => e.EventManagerId == managerId);

        public IEnumerable<Event> GetEventsByHost(int hostId) =>
            _eventRepo.GetAll().Where(e => e.EventHostId == hostId);

        public IEnumerable<Event> GetEventsByParticipant(int participantId) =>
            _eventRepo.GetAll().Where(e => e.ParticipantIds.Contains(participantId));

        public IEnumerable<Participant> GetEventParticipants(int eventId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new EntityNotFoundException("Event", eventId);
            return _participantRepo.GetAll().Where(p => ev.ParticipantIds.Contains(p.Id));
        }

        public IEnumerable<Participant> GetAllParticipants() =>
            _participantRepo.GetAll();

        public Event? GetEventByParticipantId(int participantId) =>
            _eventRepo.GetAll().FirstOrDefault(e => e.ParticipantIds.Contains(participantId));

        public void AddParticipantToEvent(int eventId, int participantId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new EntityNotFoundException("Event", eventId);
            if (ev.ParticipantIds.Contains(participantId))
                throw new InvalidEventDataException("Participant is already registered to this event.");
            if (ev.EventHostId == participantId)
                throw new InvalidEventDataException("The event host cannot be added as a participant.");
            ev.ParticipantIds.Add(participantId);
            ev.ParticipantData.Add(new EventParticipantData { ParticipantId = participantId });
            _eventRepo.Update(ev);
        }

        public void SendHostInvitation(int eventId, IMailService mailService)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new EntityNotFoundException("Event", eventId);
            var host = _participantRepo.GetById(ev.EventHostId) ?? throw new EntityNotFoundException("Host", ev.EventHostId);

            mailService.SendEmail(
                host.Email,
                $"[GatherUp] You're invited to host: {ev.Title}",
                EmailTemplates.HostInvitation(host.Name, ev.Title, ev.Date.ToString("dddd, dd MMMM yyyy"), ev.Location)
            );
        }
    }
}
