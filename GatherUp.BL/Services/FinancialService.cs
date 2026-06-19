using GatherUp.core.DO;
using GatherUp.core.Exceptions;
using GatherUp.core.interfaces;

namespace GatherUp.BL.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly IRepository<Participant> _participantRepo;
        private readonly IRepository<Event> _eventRepo;
        private readonly IMailService _mailService;

        public FinancialService(
            IRepository<Participant> participantRepo,
            IRepository<Event> eventRepo,
            IMailService mailService)
        {
            _participantRepo = participantRepo;
            _eventRepo = eventRepo;
            _mailService = mailService;
        }

        public event Action<int, int>? OnPaymentReceived;
        public event Action<int, decimal>? OnBudgetChanged;

        public void RegisterPayment(int participantId, decimal amount)
        {
            var participant = _participantRepo.GetById(participantId)
                ?? throw new NotFoundException("Participant", participantId);

            participant.HasPaid = true;
            participant.AmountContributed += amount;
            _participantRepo.Update(participant);

            var ev = _eventRepo.GetAll().FirstOrDefault(e => e.ParticipantIds.Contains(participantId));
            if (ev != null)
                OnPaymentReceived?.Invoke(ev.Id, participantId);
        }

        // Single method for both adding debt and updating vendor amount
        public void SetVendorAmount(int eventId, int vendorId, decimal amount)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new NotFoundException("Event", eventId);
            var vendor = ev.Vendors.FirstOrDefault(v => v.Id == vendorId)
                ?? throw new NotFoundException("Vendor", vendorId);

            vendor.AmountOwed = amount;
            _eventRepo.Update(ev);
            OnBudgetChanged?.Invoke(eventId, GetTotalBudget(eventId));
        }

        public decimal GetTotalBudget(int eventId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new NotFoundException("Event", eventId);
            return ev.Vendors.Sum(v => v.AmountOwed);
        }

        public void SendPaymentReminders(int eventId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new NotFoundException("Event", eventId);

            _participantRepo.GetAll()
                .Where(p => ev.ParticipantIds.Contains(p.Id) && !p.HasPaid)
                .ToList()
                .ForEach(p => _mailService.SendEmail(
                    p.Email,
                    $"Payment Reminder - {ev.Title}",
                    $"Hello {p.Name}, please complete your payment for {ev.Title}.\nBank: 12, Branch: 345, Account: 678901."
                ));
        }

        public (IEnumerable<Participant> PaidParticipants, decimal TotalIncome,
                IEnumerable<VendorAllocation> Vendors, decimal TotalOutgoing,
                decimal Balance) GetFinancialSummary(int eventId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new NotFoundException("Event", eventId);
            var paid = GetPaidParticipants(eventId);
            var income = CalculateTotalIncome(paid);
            var outgoing = CalculateTotalOutgoing(ev);

            return (paid, income, ev.Vendors, outgoing, income - outgoing);
        }

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

        public IEnumerable<(string ReceiptNumber, decimal Amount)> GetAllReceiptsSorted(int eventId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new NotFoundException("Event", eventId);
            return ev.Vendors
                .SelectMany(v => v.Receipts)
                .OrderByDescending(r => r.Date)
                .Select(r => (r.ReceiptNumber, r.Amount));
        }
    }
}
