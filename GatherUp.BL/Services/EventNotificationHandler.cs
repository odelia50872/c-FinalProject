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
            var statusText = isAttending ? "\u2713 Confirmed" : "\u2717 Declined";
            var statusColor = isAttending ? "#22c55e" : "#ef4444";

            if ((participant.MailingPreferences & MailingPreference.AttendanceConfirmation) != 0)
            {
                var body =
                    $"<p>Hi <strong>{participant.Name}</strong>,</p>" +
                    $"<p>Your attendance response has been successfully recorded:</p>" +
                    $"<div class=\"info-box\">" +
                    $"<div class=\"info-row\"><span class=\"info-label\">Event</span><span>{ev.Title}</span></div>" +
                    $"<div class=\"info-row\"><span class=\"info-label\">Date</span><span>{ev.Date:dddd, dd MMMM yyyy}</span></div>" +
                    $"<div class=\"info-row\"><span class=\"info-label\">Location</span><span>{ev.Location}</span></div>" +
                    $"<div class=\"info-row\"><span class=\"info-label\">Status</span><span style=\"color:{statusColor};font-weight:700\">{statusText}</span></div>" +
                    $"</div>" +
                    (isAttending ? "<p>We look forward to seeing you there!</p>" : "<p>You can always change your response by logging into GatherUp.</p>");

                _mailService.SendEmail(participant.Email,
                    $"[GatherUp] Attendance {(isAttending ? "confirmed" : "declined")} - {ev.Title}",
                    EmailTemplates.Build(isAttending ? "You're going! \U0001f389" : "We'll miss you \U0001f61f",
                        $"Your attendance response for \"{ev.Title}\" has been recorded.", body));
            }

            var manager = _pRepo.GetById(ev.EventManagerId);
            if (manager != null)
            {
                var body =
                    $"<p>Hi <strong>{manager.Name}</strong>,</p>" +
                    $"<p>A participant has updated their attendance status for your event:</p>" +
                    $"<div class=\"info-box\">" +
                    $"<div class=\"info-row\"><span class=\"info-label\">Event</span><span>{ev.Title}</span></div>" +
                    $"<div class=\"info-row\"><span class=\"info-label\">Participant</span><span>{participant.Name} ({participant.Email})</span></div>" +
                    $"<div class=\"info-row\"><span class=\"info-label\">Status</span><span style=\"color:{statusColor};font-weight:700\">{statusText}</span></div>" +
                    $"</div>";

                _mailService.SendEmail(manager.Email,
                    $"[GatherUp] {participant.Name} {(isAttending ? "confirmed" : "declined")} - {ev.Title}",
                    EmailTemplates.Build("Attendance Update \U0001f4cb",
                        $"{participant.Name} responded to the invitation for \"{ev.Title}\".", body));
            }
        }

        private void HandlePaymentReceived(int evId, int participantId, decimal amount)
        {
            var ev = _eRepo.GetById(evId);
            var participant = _pRepo.GetById(participantId);
            if (ev == null || participant == null) return;

            if ((participant.MailingPreferences & MailingPreference.PaymentConfirmation) != 0)
            {
                var body =
                    $"<p>Hi <strong>{participant.Name}</strong>,</p>" +
                    $"<p>Your payment has been successfully recorded by the event organizer.</p>" +
                    $"<div class=\"info-box\">" +
                    $"<div class=\"info-row\"><span class=\"info-label\">Event</span><span>{ev.Title}</span></div>" +
                    $"<div class=\"info-row\"><span class=\"info-label\">Date</span><span>{ev.Date:dddd, dd MMMM yyyy}</span></div>" +
                    $"<div class=\"info-row\"><span class=\"info-label\">Amount</span><span style=\"color:#22c55e;font-weight:700\">\u20aa{amount}</span></div>" +
                    $"<div class=\"info-row\"><span class=\"info-label\">Status</span><span style=\"color:#22c55e;font-weight:700\">\u2713 Paid</span></div>" +
                    $"</div>" +
                    $"<p>Thank you! See you at the event \U0001f389</p>";

                _mailService.SendEmail(participant.Email,
                    $"[GatherUp] Payment confirmed - {ev.Title}",
                    EmailTemplates.Build("Payment received! \U0001f4b3",
                        $"Your payment of \u20aa{amount} for \"{ev.Title}\" has been confirmed.", body));
            }

            var manager = _pRepo.GetById(ev.EventManagerId);
            if (manager != null)
            {
                var body =
                    $"<p>Hi <strong>{manager.Name}</strong>,</p>" +
                    $"<p>A participant has completed their payment for your event:</p>" +
                    $"<div class=\"info-box\">" +
                    $"<div class=\"info-row\"><span class=\"info-label\">Event</span><span>{ev.Title}</span></div>" +
                    $"<div class=\"info-row\"><span class=\"info-label\">Participant</span><span>{participant.Name} ({participant.Email})</span></div>" +
                    $"<div class=\"info-row\"><span class=\"info-label\">Amount</span><span style=\"color:#22c55e;font-weight:700\">\u20aa{amount}</span></div>" +
                    $"</div>";

                _mailService.SendEmail(manager.Email,
                    $"[GatherUp] Payment received from {participant.Name} - {ev.Title}",
                    EmailTemplates.Build("Payment received \U0001f4b0",
                        $"{participant.Name} paid \u20aa{amount} for \"{ev.Title}\".", body));
            }
        }

        private void HandlePollCreated(int pollId)
        {
            var poll = _plRepo.GetById(pollId);
            var ev = _eRepo.GetAll().FirstOrDefault(e => e.PollIds.Contains(pollId));
            if (ev == null || poll == null) return;

            _pRepo.GetAll()
                .Where(p => ev.ParticipantIds.Contains(p.Id) && (p.MailingPreferences & MailingPreference.PollResponses) != 0)
                .ToList()
                .ForEach(p =>
                {
                    var body =
                        $"<p>Hi <strong>{p.Name}</strong>,</p>" +
                        $"<p>A new poll has been created for your event. Your opinion matters!</p>" +
                        $"<div class=\"info-box\">" +
                        $"<div class=\"info-row\"><span class=\"info-label\">Event</span><span>{ev.Title}</span></div>" +
                        $"<div class=\"info-row\"><span class=\"info-label\">Poll</span><span>{poll.Name}</span></div>" +
                        $"<div class=\"info-row\"><span class=\"info-label\">Questions</span><span>{poll.Questions.Count}</span></div>" +
                        $"</div>" +
                        $"<p>Log in to GatherUp to cast your vote.</p>";

                    _mailService.SendEmail(p.Email,
                        $"[GatherUp] New poll: {poll.Name} - {ev.Title}",
                        EmailTemplates.Build("New poll available! \U0001f5f3\ufe0f",
                            $"A new poll has been created for \"{ev.Title}\".", body));
                });
        }

        private void HandleEventDetailsChanged(int evId)
        {
            var ev = _eRepo.GetById(evId);
            if (ev == null) return;

            _pRepo.GetAll()
                .Where(p => ev.ParticipantIds.Contains(p.Id) && p.MailingPreferences != MailingPreference.None)
                .ToList()
                .ForEach(p =>
                {
                    var body =
                        $"<p>Hi <strong>{p.Name}</strong>,</p>" +
                        $"<p>The organizer has updated the details for your upcoming event:</p>" +
                        $"<div class=\"info-box\">" +
                        $"<div class=\"info-row\"><span class=\"info-label\">Event</span><span>{ev.Title}</span></div>" +
                        $"<div class=\"info-row\"><span class=\"info-label\">Date</span><span>{ev.Date:dddd, dd MMMM yyyy}</span></div>" +
                        $"<div class=\"info-row\"><span class=\"info-label\">Location</span><span>{ev.Location}</span></div>" +
                        $"</div>" +
                        $"<p>Please make note of any changes. See you there! \U0001f389</p>";

                    _mailService.SendEmail(p.Email,
                        $"[GatherUp] Event updated: {ev.Title}",
                        EmailTemplates.Build("Event details updated \U0001f4dd",
                            $"The details for \"{ev.Title}\" have been updated.", body));
                });
        }
    }
}
