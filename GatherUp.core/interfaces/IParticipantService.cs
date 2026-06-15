using GatherUp.core.DO;

namespace GatherUp.core.interfaces
{
    public interface IParticipantService
    {
        void AddParticipant(Participant participant);
        void ConfirmAttendance(int participantId, bool isAttending);
        void SendPendingInvitations(int eventId);
    }
}
