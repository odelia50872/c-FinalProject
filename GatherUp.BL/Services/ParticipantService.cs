using GatherUp.core.DO;
using GatherUp.core.Exceptions;
using GatherUp.core.interfaces;
using GatherUp.Infrastructure.Services;

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

        public Participant GetById(int id) =>
            _participantRepo.GetById(id) ?? throw new EntityNotFoundException("Participant", id);

        public Participant UpdateParticipant(int id, string name, string phone)
        {
            var participant = _participantRepo.GetById(id)
                ?? throw new EntityNotFoundException("Participant", id);

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidEventDataException("Name cannot be empty.");

            participant.Name = name;
            participant.PhoneNumber = phone;
            _participantRepo.Update(participant);
            return participant;
        }

        public void ConfirmAttendance(int participantId, bool isAttending)
        {
            var participant = _participantRepo.GetById(participantId)
                ?? throw new EntityNotFoundException("Participant", participantId);

            var ev = _eventRepo.GetAll().FirstOrDefault(e => e.ParticipantIds.Contains(participantId))
                ?? throw new EntityNotFoundException("Event for participant", participantId);

            var data = ev.ParticipantData.FirstOrDefault(d => d.ParticipantId == participantId);
            if (data == null)
            {
                data = new EventParticipantData { ParticipantId = participantId };
                ev.ParticipantData.Add(data);
            }
            data.IsAttending = isAttending;
            _eventRepo.Update(ev);

            OnAttendanceConfirmed?.Invoke(ev.Id, participantId);
        }

        public void UpdateMailingPreference(int participantId, MailingPreference preference)
        {
            var participant = _participantRepo.GetById(participantId)
                ?? throw new EntityNotFoundException("Participant", participantId);

            participant.MailingPreferences = preference;
            _participantRepo.Update(participant);
        }

        public void SendPendingInvitations(int eventId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new EntityNotFoundException("Event", eventId);

            _participantRepo.GetAll()
                .Where(p => ev.ParticipantIds.Contains(p.Id)
                    && (ev.ParticipantData.FirstOrDefault(d => d.ParticipantId == p.Id)?.IsAttending == null)
                    && (p.MailingPreferences & MailingPreference.AttendanceConfirmation) != 0)
                .ToList()
                .ForEach(p => _mailService.SendEmail(
                    p.Email,
                    $"[GatherUp] Reminder: Confirm attendance for {ev.Title}",
                    EmailTemplates.PendingInvitation(p.Name, ev.Title, ev.Date.ToString("dddd, dd MMMM yyyy"), ev.Location)));
        }
    }
}
