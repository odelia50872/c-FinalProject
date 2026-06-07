using GatherUp.core.DO;
using GatherUp.Infrastructure.XML;
using System.Xml.Linq;

namespace GatherUp.Infrastructure.Repositories
{
    public class ReceiptRepository : XmlRepository<VendorAllocation>
    {
        private readonly string _xmlFilePath;
        private readonly string _receiptsFolder;

        public ReceiptRepository(string folderPath, string receiptsFolder) : base(folderPath)
        {
            _xmlFilePath = Path.Combine(folderPath, "Receipts.xml");
            _receiptsFolder = receiptsFolder;
            Directory.CreateDirectory(_receiptsFolder);
        }

        public void AddReceipt(ReceiptDetails receipt, string sourceFilePath)
        {
            string fileName = Path.GetFileName(sourceFilePath);
            string destPath = Path.Combine(_receiptsFolder, fileName);
            File.Copy(sourceFilePath, destPath, overwrite: true);

            XElement element = new XElement("Receipt",
                new XAttribute("ReceiptNumber", receipt.ReceiptNumber),
                new XElement("Amount", receipt.Amount),
                new XElement("Date", receipt.Date),
                new XElement("FilePath", destPath)
            );

            var elements = XMLDocManager.LoadElements(_xmlFilePath, "Receipt").ToList();
            elements.Add(element);
            XMLDocManager.SaveElements(_xmlFilePath, "Receipts", elements);
        }

        public ReceiptDetails? GetReceiptById(string receiptNumber)
        {
            var element = XMLDocManager.LoadElements(_xmlFilePath, "Receipt")
                .FirstOrDefault(e => e.Attribute("ReceiptNumber")?.Value == receiptNumber);

            if (element == null) return null;

            return new ReceiptDetails(
                element.Attribute("ReceiptNumber")!.Value,
                decimal.Parse(element.Element("Amount")!.Value),
                DateTime.Parse(element.Element("Date")!.Value)
            );
        }

        public override void Add(VendorAllocation entity) =>
            throw new InvalidOperationException("השתמש ב-AddReceipt במקום");

        public override void Update(VendorAllocation entity) =>
            throw new InvalidOperationException("לא ניתן לערוך קבלה לאחר היצירה");

        public override void Delete(int id) =>
            throw new InvalidOperationException("לא ניתן למחוק קבלה לאחר היצירה");
    }
}
