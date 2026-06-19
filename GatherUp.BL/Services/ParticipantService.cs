using GatherUp.core.DO;
using GatherUp.core.Exceptions;
using GatherUp.core.interfaces;

namespace GatherUp.BL.Services
{
    public class ParticipantService : IParticipantService
    {
        private readonly IRepository<Participant> _participantRepo;
        private readonly IRepository<Event> _eventRepo;
        private readonly IMailService _mailService;

        public ParticipantService(
            IRepository<Participant> participantRepo,
            IRepository<Event> eventRepo,
            IMailService mailService)
        {
            _participantRepo = participantRepo;
            _eventRepo = eventRepo;
            _mailService = mailService;
        }

        public event Action<int, int>? OnAttendanceConfirmed;

        public void AddParticipant(Participant participant) =>
            _participantRepo.Add(participant);

        public Participant GetParticipantById(int participantId) =>
            _participantRepo.GetById(participantId) ?? throw new NotFoundException("Participant", participantId);

        public void ConfirmAttendance(int participantId, bool isAttending)
        {
            var participant = _participantRepo.GetById(participantId)
                ?? throw new NotFoundException("Participant", participantId);

            participant.IsAttending = isAttending;
            _participantRepo.Update(participant);

            var ev = _eventRepo.GetAll().FirstOrDefault(e => e.ParticipantIds.Contains(participantId));
            if (ev != null)
                OnAttendanceConfirmed?.Invoke(ev.Id, participantId);
        }

        public void UpdateMailingPreference(int participantId, MailingPreference preference)
        {
            var participant = _participantRepo.GetById(participantId)
                ?? throw new NotFoundException("Participant", participantId);

            participant.MailingPreferences = preference;
            _participantRepo.Update(participant);
        }

        public void SendPendingInvitations(int eventId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new NotFoundException("Event", eventId);

            _participantRepo.GetAll()
                .Where(p => ev.ParticipantIds.Contains(p.Id) && p.IsAttending == null)
                .ToList()
                .ForEach(p => _mailService.SendEmail(
                    p.Email,
                    $"Reminder: Please confirm attendance for {ev.Title}",
                    $"Hello {p.Name}, please confirm your attendance for {ev.Title} on {ev.Date:dd/MM/yyyy}."
                ));
        }
    }
}
