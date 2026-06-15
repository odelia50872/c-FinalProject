using GatherUp.core.DO;
using GatherUp.core.interfaces;

namespace GatherUp.BL.Services
{
    public class EventService : IEventService
    {
        private readonly IRepository<Event> _eventRepo;
        private readonly IRepository<Participant> _participantRepo;

        public EventService(IRepository<Event> eventRepo, IRepository<Participant> participantRepo)
        {
            _eventRepo = eventRepo;
            _participantRepo = participantRepo;
        }

        public event Action<int, int>? OnAttendanceConfirmed;
        public event Action<int, int>? OnPaymentReceived;
        public event Action<int, int>? OnPollAnswerSubmitted;
        public event Action<int>? OnPollCreated;
        public event Action<int>? OnEventDetailsChanged;
        public event Action<int, decimal>? OnBudgetChanged;

        public void RaiseAttendanceConfirmed(int eventId, int participantId) =>
            OnAttendanceConfirmed?.Invoke(eventId, participantId);

        public void RaisePaymentReceived(int eventId, int participantId) =>
            OnPaymentReceived?.Invoke(eventId, participantId);

        public void RaisePollAnswerSubmitted(int eventId, int pollId) =>
            OnPollAnswerSubmitted?.Invoke(eventId, pollId);

        public void RaisePollCreated(int pollId) =>
            OnPollCreated?.Invoke(pollId);

        public void RaiseEventDetailsChanged(int eventId) =>
            OnEventDetailsChanged?.Invoke(eventId);

        public IEnumerable<Event> GetEventsByParticipant(int participantId) =>
            _eventRepo.GetAll().Where(e => e.ParticipantIds.Contains(participantId));

        public IEnumerable<string> GetEventTitlesByParticipant(int participantId) =>
            GetEventsByParticipant(participantId).Select(e => e.Title);

        public IEnumerable<Participant> GetEventParticipants(int eventId)
        {
            var ev = _eventRepo.GetById(eventId);
            if (ev == null) return Enumerable.Empty<Participant>();
            return _participantRepo.GetAll().Where(p => ev.ParticipantIds.Contains(p.Id));
        }

        public decimal GetTotalBudget(int eventId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new Exception("אירוע לא נמצא");
            return ev.Vendors.Sum(v => v.AmountOwed);
        }

        public void UpdateVendorAmount(int eventId, int vendorId, decimal newAmount)
        {
            var ev = _eventRepo.GetById(eventId);
            var vendor = ev?.Vendors.FirstOrDefault(v => v.Id == vendorId);
            if (vendor == null) return;

            vendor.AmountOwed = newAmount;
            _eventRepo.Update(ev!);
            OnBudgetChanged?.Invoke(eventId, GetTotalBudget(eventId));
            OnEventDetailsChanged?.Invoke(eventId);
        }

        public void AddParticipantToEvent(int eventId, int participantId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new Exception("האירוע לא קיים");
            if (ev.ParticipantIds.Contains(participantId))
                throw new Exception("המשתתף כבר רשום לאירוע");
            ev.ParticipantIds.Add(participantId);
            _eventRepo.Update(ev);
        }
    }
}
