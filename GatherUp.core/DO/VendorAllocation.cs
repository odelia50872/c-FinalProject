using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatherUp.core.interfaces;

namespace GatherUp.core.DO
{
    public class VendorAllocation:IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal AmountOwed { get; set; }
        public bool HasReceipt { get; set; }
        public List<ReceiptDetails> Receipts { get; set; } = new List<ReceiptDetails>();
    }
}
