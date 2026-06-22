using GatherUp.core.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GatherUp.core.DO
{
    public class VendorAllocation : IEntity
    {
        public VendorAllocation()
        {
            Receipts = new List<ReceiptDetails>();
        }
        [XmlAttribute]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal AmountOwed { get; set; }
        public bool HasReceipt { get; set; }
        public List<ReceiptDetails> Receipts { get; set; } 
    }
}
