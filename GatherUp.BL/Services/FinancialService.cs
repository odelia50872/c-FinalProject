using GatherUp.core.DO;
using GatherUp.core.Exceptions;
using GatherUp.core.interfaces;
using GatherUp.Infrastructure.Services;

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

        public event Action<int, int, decimal>? OnPaymentReceived;
        public event Action<int, decimal>? OnBudgetChanged;

        public void RegisterPayment(int participantId, decimal amount)
        {
            _ = _participantRepo.GetById(participantId)
                ?? throw new EntityNotFoundException("Participant", participantId);

            var ev = _eventRepo.GetAll().FirstOrDefault(e => e.ParticipantIds.Contains(participantId))
                ?? throw new EntityNotFoundException("Event for participant", participantId);

            var data = ev.ParticipantData.FirstOrDefault(d => d.ParticipantId == participantId);
            if (data == null)
            {
                data = new EventParticipantData { ParticipantId = participantId };
                ev.ParticipantData.Add(data);
            }
            data.HasPaid = true;
            data.AmountContributed += amount;
            _eventRepo.Update(ev);

            OnPaymentReceived?.Invoke(ev.Id, participantId, amount);
        }

        public void SetVendorAmount(int eventId, int vendorId, decimal amount)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new EntityNotFoundException("Event", eventId);
            var vendor = ev.Vendors.FirstOrDefault(v => v.Id == vendorId)
                ?? throw new EntityNotFoundException("Vendor", vendorId);

            vendor.AmountOwed = amount;
            _eventRepo.Update(ev);
            OnBudgetChanged?.Invoke(eventId, GetTotalBudget(eventId));
        }

        public decimal GetTotalBudget(int eventId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new EntityNotFoundException("Event", eventId);
            return ev.Vendors.Sum(v => v.AmountOwed);
        }

        public int SendPaymentReminders(int eventId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new EntityNotFoundException("Event", eventId);

            var unpaid = _participantRepo.GetAll()
                .Where(p => ev.ParticipantIds.Contains(p.Id)
                    && !(ev.ParticipantData.FirstOrDefault(d => d.ParticipantId == p.Id)?.HasPaid ?? false)
                    && (p.MailingPreferences & MailingPreference.PaymentConfirmation) != 0)
                .ToList();

            unpaid.ForEach(p =>
                _mailService.SendEmail(
                    p.Email,
                    $"[GatherUp] Payment reminder - {ev.Title}",
                    EmailTemplates.PaymentReminder(p.Name, ev.Title, ev.Date.ToString("dddd, dd MMMM yyyy"), ev.Location)
                ));

            return unpaid.Count;
        }

        public (IEnumerable<(Participant Participant, decimal AmountContributed)> PaidParticipants, decimal TotalIncome, IEnumerable<VendorAllocation> Vendors, decimal TotalOutgoing, decimal Balance) GetFinancialSummary(int eventId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new EntityNotFoundException("Event", eventId);

            var paidData = ev.ParticipantData.Where(d => d.HasPaid && d.IsAttending == true).ToList();
            var paidParticipants = paidData
                .Where(d => _participantRepo.GetById(d.ParticipantId) != null)
                .Select(d => (Participant: _participantRepo.GetById(d.ParticipantId)!, d.AmountContributed))
                .ToList();

            var income = paidData.Sum(d => d.AmountContributed);
            var outgoing = ev.Vendors.Sum(v => v.AmountOwed);

            return (paidParticipants, income, ev.Vendors, outgoing, income - outgoing);
        }

        public IEnumerable<(string ReceiptNumber, decimal Amount)> GetAllReceiptsSorted(int eventId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new EntityNotFoundException("Event", eventId);
            return ev.Vendors
                .SelectMany(v => v.Receipts)
                .OrderByDescending(r => r.Date)
                .Select(r => (r.ReceiptNumber, r.Amount));
        }
    }
}
