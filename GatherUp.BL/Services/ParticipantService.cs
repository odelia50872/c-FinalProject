using GatherUp.core.DO;
using GatherUp.core.interfaces;

namespace GatherUp.BL.Services
{
    public class ParticipantService : IParticipantService
    {
        private readonly IRepository<Participant> _participantRepo;
        private readonly IRepository<Event> _eventRepo;
        private readonly IMailService _mailService;
        private readonly IEventService _eventService;

        public ParticipantService(
            IRepository<Participant> participantRepo,
            IRepository<Event> eventRepo,
            IMailService mailService,
            IEventService eventService)
        {
            _participantRepo = participantRepo;
            _eventRepo = eventRepo;
            _mailService = mailService;
            _eventService = eventService;
        }

        public void AddParticipant(Participant participant)
        {
            _participantRepo.Add(participant);
        }

        public void ConfirmAttendance(int participantId, bool isAttending)
        {
            var participant = _participantRepo.GetById(participantId)
                ?? throw new Exception("משתתף לא נמצא");

            participant.IsAttending = isAttending;
            _participantRepo.Update(participant);

            var ev = _eventRepo.GetAll().FirstOrDefault(e => e.ParticipantIds.Contains(participantId));
            if (ev != null)
                _eventService.RaiseAttendanceConfirmed(ev.Id, participantId);
        }

        public void SendPendingInvitations(int eventId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new Exception("אירוע לא נמצא");

            _participantRepo.GetAll()
                .Where(p => ev.ParticipantIds.Contains(p.Id) && p.IsAttending == null)
                .ToList()
                .ForEach(p => _mailService.SendEmail(
                    p.Email,
                    $"תזכורת: אישור הגעה לאירוע {ev.Title}",
                    $"שלום {p.Name},\nנשמח לדעת האם תוכל/י להגיע לאירוע {ev.Title} בתאריך {ev.Date:dd/MM/yyyy}.\nנא לאשר הגעה בהקדם."
                ));
        }
    }
}
