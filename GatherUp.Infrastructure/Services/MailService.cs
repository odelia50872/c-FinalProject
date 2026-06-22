using GatherUp.core.interfaces;
using System.Net;
using System.Net.Mail;

namespace GatherUp.Infrastructure.Services
{
    public class MailService : IMailService
    {
        private readonly string _logFilePath;
        private readonly SmtpSettings? _smtp;

        public MailService(string logFilePath = "MailLog.txt", SmtpSettings? smtp = null)
        {
            _logFilePath = logFilePath;
            _smtp = smtp;
        }

        public void SendEmail(string to, string subject, string body)
        {
            File.AppendAllText(_logFilePath, $@"
--------------------------------------------------
Date: {DateTime.Now}
To: {to}
Subject: {subject}
Body (HTML): {body}
--------------------------------------------------
");

            if (_smtp is { Host: not null, Username: not null, Password: not null }
                && !string.IsNullOrWhiteSpace(_smtp.Username)
                && !string.IsNullOrWhiteSpace(_smtp.Password))
            {
                try
                {
                    using var client = new SmtpClient(_smtp.Host, _smtp.Port)
                    {
                        Credentials = new NetworkCredential(_smtp.Username, _smtp.Password),
                        EnableSsl = true
                    };
                    var message = new MailMessage(_smtp.Username, to, subject, body)
                    {
                        IsBodyHtml = true
                    };
                    client.Send(message);
                }
                catch (Exception ex)
                {
                    File.AppendAllText(_logFilePath, $"[SMTP ERROR] {ex.Message}\n");
                }
            }
        }
    }

    public class SmtpSettings
    {
        public string? Host { get; set; }
        public int Port { get; set; } = 587;
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
