using GatherUp.core.DO;

namespace GatherUp.core.interfaces
{
    public interface IVendorService
    {
        VendorAllocation AddVendor(int eventId, string name, decimal amountOwed);
        void AddReceipt(int eventId, int vendorId, ReceiptDetails receipt);
        IEnumerable<VendorAllocation> GetVendors(int eventId);
    }
}
