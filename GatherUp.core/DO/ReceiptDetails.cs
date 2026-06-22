namespace GatherUp.core.DO
{
    public class ReceiptDetails
    {
        public ReceiptDetails() { }

        public ReceiptDetails(string receiptNumber, decimal amount, DateTime date, string filePath = "")
        {
            _receiptNumber = receiptNumber;
            _amount = amount;
            _date = date;
            FilePath = filePath;
        }

        private string _receiptNumber = string.Empty;
        public string ReceiptNumber
        {
            get => _receiptNumber;
            set { if (_receiptNumber == string.Empty) _receiptNumber = value; }
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set { if (_amount == 0) _amount = value; }
        }

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set { if (_date == default) _date = value; }
        }

        // Original file path — copied to receipts folder upon saving
        public string FilePath { get; set; } = string.Empty;
    }
}
