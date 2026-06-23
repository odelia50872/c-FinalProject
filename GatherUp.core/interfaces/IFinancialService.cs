using GatherUp.core.DO;

namespace GatherUp.core.interfaces
{
    public interface IFinancialService
    {
        event Action<int, int, decimal>? OnPaymentReceived;
        event Action<int, decimal>? OnBudgetChanged;

        void RegisterPayment(int participantId, decimal amount);
        void SetVendorAmount(int eventId, int vendorId, decimal amount);
        int SendPaymentReminders(int eventId);
        decimal GetTotalBudget(int eventId);
        (IEnumerable<(Participant Participant, decimal AmountContributed)> PaidParticipants, decimal TotalIncome, IEnumerable<VendorAllocation> Vendors, decimal TotalOutgoing, decimal Balance) GetFinancialSummary(int eventId);
        IEnumerable<(string ReceiptNumber, decimal Amount)> GetAllReceiptsSorted(int eventId);
    }
}
