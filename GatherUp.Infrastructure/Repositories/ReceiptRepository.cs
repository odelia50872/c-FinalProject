using GatherUp.core.DO;
using GatherUp.core.Exceptions;
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

        // receipt.FilePath contains the source path — copied to receipts folder here
        public override void Add(VendorAllocation entity)
        {
            foreach (var receipt in entity.Receipts)
                SaveReceiptFile(receipt);
        }

        public void AddReceipt(ReceiptDetails receipt)
        {
            string destPath = SaveReceiptFile(receipt);

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

        public IEnumerable<ReceiptDetails> GetAllReceipts()
        {
            return XMLDocManager.LoadElements(_xmlFilePath, "Receipt")
                .Select(e => new ReceiptDetails(
                    e.Attribute("ReceiptNumber")!.Value,
                    decimal.Parse(e.Element("Amount")!.Value),
                    DateTime.Parse(e.Element("Date")!.Value),
                    e.Element("FilePath")?.Value ?? string.Empty
                ));
        }

        public ReceiptDetails? GetReceiptById(string receiptNumber)
        {
            var element = XMLDocManager.LoadElements(_xmlFilePath, "Receipt")
                .FirstOrDefault(e => e.Attribute("ReceiptNumber")?.Value == receiptNumber);

            if (element == null) return null;

            return new ReceiptDetails(
                element.Attribute("ReceiptNumber")!.Value,
                decimal.Parse(element.Element("Amount")!.Value),
                DateTime.Parse(element.Element("Date")!.Value),
                element.Element("FilePath")?.Value ?? string.Empty
            );
        }

        public override void Update(VendorAllocation entity) =>
            throw new ImmutableReceiptException();

        public override void Delete(int id) =>
            throw new ImmutableReceiptException();

        private string SaveReceiptFile(ReceiptDetails receipt)
        {
            if (string.IsNullOrEmpty(receipt.FilePath) || !File.Exists(receipt.FilePath))
                return string.Empty;

            string fileName = Path.GetFileName(receipt.FilePath);
            string destPath = Path.Combine(_receiptsFolder, fileName);
            File.Copy(receipt.FilePath, destPath, overwrite: true);
            receipt.FilePath = destPath;
            return destPath;
        }
    }
}
