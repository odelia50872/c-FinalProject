using GatherUp.core.DO;
using GatherUp.core.Exceptions;
using GatherUp.core.interfaces;

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
            _eventRepo.GetById(eventId) ?? throw new NotFoundException("Event", eventId);

        public bool IsManager(int eventId, int userId) =>
            _eventRepo.GetById(eventId)?.EventManagerId == userId;

        public Event CreateEvent(string title, DateTime date, string location, int managerId, int hostId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ValidationException("Event title cannot be empty.");

            var newId = _eventRepo.GetAll().Any() ? _eventRepo.GetAll().Max(e => e.Id) + 1 : 1;
            var ev = new Event
            {
                Id = newId,
                Title = title,
                Date = date,
                Location = location,
                EventManagerId = managerId,
                EventHostId = hostId
            };
            _eventRepo.Add(ev);
            return ev;
        }

        public void UpdateEvent(int eventId, string title, DateTime date, string location)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new NotFoundException("Event", eventId);
            if (string.IsNullOrWhiteSpace(title))
                throw new ValidationException("Event title cannot be empty.");

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
            var ev = _eventRepo.GetById(eventId) ?? throw new NotFoundException("Event", eventId);
            return _participantRepo.GetAll().Where(p => ev.ParticipantIds.Contains(p.Id));
        }

        public void AddParticipantToEvent(int eventId, int participantId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new NotFoundException("Event", eventId);
            if (ev.ParticipantIds.Contains(participantId))
                throw new ValidationException("Participant is already registered to this event.");
            ev.ParticipantIds.Add(participantId);
            _eventRepo.Update(ev);
        }
    }
}
