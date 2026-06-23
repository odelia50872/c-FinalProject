using GatherUp.core.DO;
using GatherUp.core.interfaces;
using GatherUp.Infrastructure.Services;

namespace GatherUp.BL.Services
{
    public class EventNotificationHandler
    {
        private readonly IRepository<Participant> _pRepo;
        private readonly IRepository<Event> _eRepo;
        private readonly IRepository<Poll> _plRepo;
        private readonly IMailService _mailService;

        public EventNotificationHandler(
            IRepository<Participant> pRepo,
            IRepository<Event> eRepo,
            IRepository<Poll> plRepo,
            IMailService mailService,
            IParticipantService participantService,
            IFinancialService financialService,
            IPollService pollService,
            IEventService eventService)
        {
            _pRepo = pRepo;
            _eRepo = eRepo;
            _plRepo = plRepo;
            _mailService = mailService;

            participantService.OnAttendanceConfirmed += HandleAttendanceConfirmed;
            financialService.OnPaymentReceived += HandlePaymentReceived;
            pollService.OnPollCreated += HandlePollCreated;
            eventService.OnEventDetailsChanged += HandleEventDetailsChanged;
        }

        private void HandleAttendanceConfirmed(int evId, int participantId)
        {
            var ev = _eRepo.GetById(evId);
            var participant = _pRepo.GetById(participantId);
            if (ev == null || participant == null) return;

            var data = ev.ParticipantData.FirstOrDefault(d => d.ParticipantId == participantId);
            var isAttending = data?.IsAttending == true;

            if ((participant.MailingPreferences & MailingPreference.AttendanceConfirmation) != 0)
                _mailService.SendEmail(participant.Email,
                    $"[GatherUp] Attendance {(isAttending ? "confirmed" : "declined")} - {ev.Title}",
                    EmailTemplates.AttendanceConfirmed(participant.Name, ev.Title, ev.Date.ToString("dddd, dd MMMM yyyy"), ev.Location, isAttending));

            var manager = _pRepo.GetById(ev.EventManagerId);
            if (manager != null)
                _mailService.SendEmail(manager.Email,
                    $"[GatherUp] {participant.Name} {(isAttending ? "confirmed" : "declined")} - {ev.Title}",
                    EmailTemplates.AttendanceManagerNotification(manager.Name, participant.Name, participant.Email, ev.Title, isAttending));
        }

        private void HandlePaymentReceived(int evId, int participantId, decimal amount)
        {
            var ev = _eRepo.GetById(evId);
            var participant = _pRepo.GetById(participantId);
            if (ev == null || participant == null) return;

            if ((participant.MailingPreferences & MailingPreference.PaymentConfirmation) != 0)
                _mailService.SendEmail(participant.Email,
                    $"[GatherUp] Payment confirmed - {ev.Title}",
                    EmailTemplates.PaymentConfirmed(participant.Name, ev.Title, ev.Date.ToString("dddd, dd MMMM yyyy"), amount));

            var manager = _pRepo.GetById(ev.EventManagerId);
            if (manager != null)
                _mailService.SendEmail(manager.Email,
                    $"[GatherUp] Payment received from {participant.Name} - {ev.Title}",
                    EmailTemplates.PaymentManagerNotification(manager.Name, participant.Name, participant.Email, ev.Title, amount));
        }

        private void HandlePollCreated(int pollId)
        {
            var poll = _plRepo.GetById(pollId);
            var ev = _eRepo.GetAll().FirstOrDefault(e => e.PollIds.Contains(pollId));
            if (ev == null || poll == null) return;

            _pRepo.GetAll()
                .Where(p => ev.ParticipantIds.Contains(p.Id) && (p.MailingPreferences & MailingPreference.PollResponses) != 0)
                .ToList()
                .ForEach(p => _mailService.SendEmail(p.Email,
                    $"[GatherUp] New poll: {poll.Name} - {ev.Title}",
                    EmailTemplates.PollCreated(p.Name, ev.Title, poll.Name, poll.Questions.Count)));
        }

        private void HandleEventDetailsChanged(int evId)
        {
            var ev = _eRepo.GetById(evId);
            if (ev == null) return;

            _pRepo.GetAll()
                .Where(p => ev.ParticipantIds.Contains(p.Id) && p.MailingPreferences != MailingPreference.None)
                .ToList()
                .ForEach(p => _mailService.SendEmail(p.Email,
                    $"[GatherUp] Event updated: {ev.Title}",
                    EmailTemplates.EventDetailsChanged(p.Name, ev.Title, ev.Date.ToString("dddd, dd MMMM yyyy"), ev.Location)));
        }
    }
}
