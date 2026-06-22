using GatherUp.core.DO;
using GatherUp.core.Exceptions;
using GatherUp.core.interfaces;

namespace GatherUp.BL.Services
{
    public class VendorService : IVendorService
    {
        private readonly IRepository<Event> _eventRepo;

        public VendorService(IRepository<Event> eventRepo)
        {
            _eventRepo = eventRepo;
        }

        // Vendor management screen - "Add Vendor"
        public VendorAllocation AddVendor(int eventId, string name, decimal amountOwed)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new EntityNotFoundException("Event", eventId);

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidEventDataException("Vendor name cannot be empty.");

            var newId = ev.Vendors.Any() ? ev.Vendors.Max(v => v.Id) + 1 : 1;
            var vendor = new VendorAllocation { Id = newId, Name = name, AmountOwed = amountOwed };

            ev.Vendors.Add(vendor);
            _eventRepo.Update(ev);
            return vendor;
        }

        // Vendor management screen - "Add Receipt"
        public void AddReceipt(int eventId, int vendorId, ReceiptDetails receipt)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new EntityNotFoundException("Event", eventId);
            var vendor = ev.Vendors.FirstOrDefault(v => v.Id == vendorId)
                ?? throw new EntityNotFoundException("Vendor", vendorId);

            if (vendor.Receipts.Any(r => r.ReceiptNumber == receipt.ReceiptNumber))
                throw new ImmutableReceiptException();

            vendor.Receipts.Add(receipt);
            vendor.HasReceipt = true;
            _eventRepo.Update(ev);
        }

        // Vendor management screen - view vendors list
        public IEnumerable<VendorAllocation> GetVendors(int eventId)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new EntityNotFoundException("Event", eventId);
            return ev.Vendors;
        }
    }
}
