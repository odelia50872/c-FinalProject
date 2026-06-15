using GatherUp.core.DO;
using GatherUp.core.interfaces;

namespace GatherUp.BL.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly IRepository<Participant> _participantRepo;
        private readonly IRepository<Event> _eventRepo;
        private readonly IMailService _mailService;
        private readonly IEventService _eventService;

        public FinancialService(
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


        public void RegisterPayment(int participantId, decimal amount)
        {
            var participant = _participantRepo.GetById(participantId)
                ?? throw new Exception("משתתף לא נמצא");

            participant.HasPaid = true;
            participant.AmountContributed += amount;
            _participantRepo.Update(participant);

            var ev = _eventRepo.GetAll().FirstOrDefault(e => e.ParticipantIds.Contains(participantId));
            if (ev != null)
                _eventService.RaisePaymentReceived(ev.Id, participantId);
        }

        public void AddVendorDebt(int eventId, int vendorId, decimal amount)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new Exception("אירוע לא נמצא");
            var vendor = ev.Vendors.FirstOrDefault(v => v.Id == vendorId)
                ?? throw new Exception("ספק לא נמצא");

            vendor.AmountOwed += amount;
            _eventRepo.Update(ev);
        }

        public void SendPaymentReminders(int eventId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new Exception("אירוע לא נמצא");

            _participantRepo.GetAll()
                .Where(p => ev.ParticipantIds.Contains(p.Id) && !p.HasPaid)
                .ToList()
                .ForEach(p => _mailService.SendEmail(
                    p.Email,
                    $"תזכורת: תשלום עבור אירוע {ev.Title}",
                    $"שלום {p.Name},\nנא לשלם עבור השתתפותך באירוע {ev.Title}.\nפרטי חשבון: בנק 12, סניף 345, חשבון 678901."
                ));
        }

        public (IEnumerable<Participant> PaidParticipants, decimal TotalIncome,
                IEnumerable<VendorAllocation> Vendors, decimal TotalOutgoing,
                decimal Balance) GetFinancialSummary(int eventId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new Exception("אירוע לא נמצא");
            var paidParticipants = GetPaidParticipants(eventId);
            var totalIncome = CalculateTotalIncome(paidParticipants);
            var totalOutgoing = CalculateTotalOutgoing(ev);

            return (paidParticipants, totalIncome, ev.Vendors, totalOutgoing, totalIncome - totalOutgoing);
        }

        public IEnumerable<(string ReceiptNumber, decimal Amount)> GetAllReceiptsSorted(int eventId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new Exception("אירוע לא נמצא");

            return ev.Vendors
                .SelectMany(v => v.Receipts)
                .OrderByDescending(r => r.Date)
                .Select(r => (r.ReceiptNumber, r.Amount));
        }

=
        private IEnumerable<Participant> GetPaidParticipants(int eventId)
        {
            var ev = _eventRepo.GetById(eventId)!;
            return _participantRepo.GetAll()
                .Where(p => ev.ParticipantIds.Contains(p.Id) && p.HasPaid && p.IsAttending == true);
        }

        private decimal CalculateTotalIncome(IEnumerable<Participant> paidParticipants) =>
            paidParticipants.Sum(p => p.AmountContributed);

        private decimal CalculateTotalOutgoing(Event ev) =>
            ev.Vendors.Sum(v => v.AmountOwed);
    }
}
