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

        public Participant GetParticipantById(int participantId) =>
            _participantRepo.GetById(participantId) ?? throw new EntityNotFoundException("Participant", participantId);

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
                .ForEach(p =>
                {
                    var bodyContent =
                        $"<p>Hi <strong>{p.Name}</strong>,</p>" +
                        $"<p>You've been invited to the following event. Please let the organizer know if you'll be attending.</p>" +
                        $"<div class=\"info-box\">" +
                        $"  <div class=\"info-row\"><span class=\"info-label\">Event</span><span>{ev.Title}</span></div>" +
                        $"  <div class=\"info-row\"><span class=\"info-label\">Date</span><span>{ev.Date:dddd, dd MMMM yyyy}</span></div>" +
                        $"  <div class=\"info-row\"><span class=\"info-label\">Location</span><span>{ev.Location}</span></div>" +
                        $"</div>" +
                        $"<p>Please log in to GatherUp and confirm your attendance as soon as possible.</p>" +
                        $"<p style=\"font-size:13px;color:#94a3b8\">If you have already responded, please ignore this reminder.</p>";

                    var body = EmailTemplates.Build(
                        "Will you be attending? \U0001f389",
                        $"Hello {p.Name}, please confirm your attendance for \"{ev.Title}\".",
                        bodyContent
                    );

                    _mailService.SendEmail(
                        p.Email,
                        $"[GatherUp] Reminder: Confirm attendance for {ev.Title}",
                        body
                    );
                });
        }
    }
}
