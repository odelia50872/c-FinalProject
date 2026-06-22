using GatherUp.core.DO;

namespace GatherUp.core.interfaces
{
    public interface IEventService
    {
        event Action<int>? OnEventDetailsChanged;

        Event CreateEvent(string title, DateTime date, string location, int managerId, int hostId);
        void UpdateEvent(int eventId, string title, DateTime date, string location);
        void AddParticipantToEvent(int eventId, int participantId);
        void SendHostInvitation(int eventId, IMailService mailService);
        IEnumerable<Event> GetEventsByParticipant(int participantId);
        IEnumerable<Event> GetEventsByManager(int managerId);
        IEnumerable<Event> GetEventsByHost(int hostId);
        IEnumerable<Participant> GetEventParticipants(int eventId);
        IEnumerable<Participant> GetAllParticipants();
        Event GetEventById(int eventId);
        bool IsManager(int eventId, int userId);
        Event? GetEventByParticipantId(int participantId);
    }
}
