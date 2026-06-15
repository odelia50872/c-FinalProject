using GatherUp.core.DO;

namespace GatherUp.core.interfaces
{
    public interface IEventService
    {
        event Action<int, int>? OnAttendanceConfirmed;     
        event Action<int, int>? OnPaymentReceived;         
        event Action<int, int>? OnPollAnswerSubmitted;     
        event Action<int>? OnPollCreated;                  
        event Action<int>? OnEventDetailsChanged;          
        event Action<int, decimal>? OnBudgetChanged;        

        decimal GetTotalBudget(int eventId);
        void UpdateVendorAmount(int eventId, int vendorId, decimal newAmount);
        void AddParticipantToEvent(int eventId, int participantId);
        IEnumerable<Event> GetEventsByParticipant(int participantId);
        IEnumerable<string> GetEventTitlesByParticipant(int participantId);
        IEnumerable<Participant> GetEventParticipants(int eventId);
        void RaiseAttendanceConfirmed(int eventId, int participantId);
        void RaisePaymentReceived(int eventId, int participantId);
        void RaisePollAnswerSubmitted(int eventId, int pollId);
        void RaisePollCreated(int pollId);
        void RaiseEventDetailsChanged(int eventId);
    }
}
