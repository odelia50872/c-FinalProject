using GatherUp.core.DO;

namespace GatherUp.core.interfaces
{
    public interface IFinancialService
    {
        void RegisterPayment(int participantId, decimal amount);
        void AddVendorDebt(int eventId, int vendorId, decimal amount);
        void SendPaymentReminders(int eventId);
        (IEnumerable<Participant> PaidParticipants, decimal TotalIncome, IEnumerable<VendorAllocation> Vendors, decimal TotalOutgoing, decimal Balance) GetFinancialSummary(int eventId);
        IEnumerable<(string ReceiptNumber, decimal Amount)> GetAllReceiptsSorted(int eventId);
    }
}
