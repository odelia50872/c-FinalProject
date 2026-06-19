using GatherUp.core.DO;

namespace GatherUp.core.interfaces
{
    public interface IParticipantService
    {
        event Action<int, int>? OnAttendanceConfirmed;

        void AddParticipant(Participant participant);
        Participant GetParticipantById(int participantId);
        void ConfirmAttendance(int participantId, bool isAttending);
        void UpdateMailingPreference(int participantId, MailingPreference preference);
        void SendPendingInvitations(int eventId);
    }
}
