using GatherUp.core.interfaces;
using System.Xml.Serialization;

namespace GatherUp.core.DO
{
    public class ReceiptDetails : IEntity
    {
        public ReceiptDetails() { }

        public ReceiptDetails(string receiptNumber, decimal amount, DateTime date, string filePath = "")
        {
            ReceiptNumber = receiptNumber;
            Amount = amount;
            Date = date;
            FilePath = filePath;
        }

        [XmlAttribute]
        public int Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string FilePath { get; set; } = string.Empty;
    }
}
