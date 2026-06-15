using GatherUp.core.interfaces;

namespace GatherUp.Infrastructure.Services
{
    public class MailService : IMailService
    {
        private readonly string _logFilePath;

        public MailService(string logFilePath = "MailLog.txt")
        {
            _logFilePath = logFilePath;
        }

        public void SendEmail(string to, string subject, string body)
        {
            string logContent = $@"
--------------------------------------------------
Date: {DateTime.Now}
To: {to}
Subject: {subject}
Body: {body}
--------------------------------------------------
";
            File.AppendAllText(_logFilePath, logContent);
            Console.WriteLine($"[MailService] Email logged to: {_logFilePath}");
        }
    }
}
