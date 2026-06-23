namespace GatherUp.Infrastructure.Services
{
    public static class EmailTemplates
    {
        public static string Build(string title, string preheader, string bodyContent)
        {
            return $@"<!DOCTYPE html>
<html>
<head>
<meta charset=""UTF-8"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
<style>
  body {{ margin:0; padding:0; background:#f4f6fb; font-family:'Segoe UI',Arial,sans-serif; }}
  .wrapper {{ max-width:600px; margin:40px auto; background:white; border-radius:16px; overflow:hidden; box-shadow:0 4px 24px rgba(0,0,0,0.08); }}
  .header {{ background:linear-gradient(135deg,#5B6EF5,#8b5cf6); padding:32px 40px; text-align:center; }}
  .header h1 {{ color:white; margin:0 0 4px; font-size:24px; font-weight:800; letter-spacing:-0.5px; }}
  .header p {{ color:rgba(255,255,255,0.8); margin:0; font-size:14px; }}
  .body {{ padding:36px 40px; }}
  .body h2 {{ font-size:20px; color:#1e293b; margin:0 0 6px; font-weight:700; }}
  .preheader {{ font-size:14px; color:#64748b; margin-bottom:24px; }}
  .body p {{ font-size:15px; color:#334155; line-height:1.7; margin:0 0 16px; }}
  .info-box {{ background:#f8fafc; border:1px solid #e2e8f0; border-radius:12px; padding:20px 24px; margin:20px 0; }}
  .info-row {{ display:flex; gap:10px; margin-bottom:10px; font-size:14px; color:#334155; }}
  .info-row:last-child {{ margin-bottom:0; }}
  .info-label {{ font-weight:600; min-width:90px; color:#64748b; }}
  .cta-btn {{ display:inline-block; background:linear-gradient(135deg,#5B6EF5,#8b5cf6); color:white !important; text-decoration:none; padding:14px 32px; border-radius:10px; font-size:15px; font-weight:600; margin:20px 0; }}
  .footer {{ background:#f8fafc; border-top:1px solid #e2e8f0; padding:20px 40px; text-align:center; font-size:12px; color:#94a3b8; }}
</style>
</head>
<body>
<div class=""wrapper"">
  <div class=""header"">
    <h1>GatherUp</h1>
    <p>Event Management System</p>
  </div>
  <div class=""body"">
    <h2>{title}</h2>
    <p class=""preheader"">{preheader}</p>
    {bodyContent}
  </div>
  <div class=""footer"">
    <p>&copy; {DateTime.Now.Year} GatherUp &bull; Event Management System</p>
    <p>This email was sent automatically. Please do not reply.</p>
  </div>
</div>
</body>
</html>";
        }

        public static string AttendanceConfirmed(string participantName, string eventTitle, string eventDate, string eventLocation, bool isAttending)
        {
            var statusText = isAttending ? "✓ Confirmed" : "✗ Declined";
            var statusColor = isAttending ? "#22c55e" : "#ef4444";
            var body =
                $"<p>Hi <strong>{participantName}</strong>,</p>" +
                $"<p>Your attendance response has been successfully recorded:</p>" +
                $"<div class=\"info-box\">" +
                $"<div class=\"info-row\"><span class=\"info-label\">Event</span><span>{eventTitle}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Date</span><span>{eventDate}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Location</span><span>{eventLocation}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Status</span><span style=\"color:{statusColor};font-weight:700\">{statusText}</span></div>" +
                $"</div>" +
                (isAttending ? "<p>We look forward to seeing you there!</p>" : "<p>You can always change your response by logging into GatherUp.</p>");
            return Build(isAttending ? "You're going! 🎉" : "We'll miss you 😟",
                $"Your attendance response for \"{eventTitle}\" has been recorded.", body);
        }

        public static string AttendanceManagerNotification(string managerName, string participantName, string participantEmail, string eventTitle, bool isAttending)
        {
            var statusText = isAttending ? "✓ Confirmed" : "✗ Declined";
            var statusColor = isAttending ? "#22c55e" : "#ef4444";
            var body =
                $"<p>Hi <strong>{managerName}</strong>,</p>" +
                $"<p>A participant has updated their attendance status for your event:</p>" +
                $"<div class=\"info-box\">" +
                $"<div class=\"info-row\"><span class=\"info-label\">Event</span><span>{eventTitle}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Participant</span><span>{participantName} ({participantEmail})</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Status</span><span style=\"color:{statusColor};font-weight:700\">{statusText}</span></div>" +
                $"</div>";
            return Build("Attendance Update 📋",
                $"{participantName} responded to the invitation for \"{eventTitle}\".", body);
        }

        public static string PaymentConfirmed(string participantName, string eventTitle, string eventDate, decimal amount)
        {
            var body =
                $"<p>Hi <strong>{participantName}</strong>,</p>" +
                $"<p>Your payment has been successfully recorded by the event organizer.</p>" +
                $"<div class=\"info-box\">" +
                $"<div class=\"info-row\"><span class=\"info-label\">Event</span><span>{eventTitle}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Date</span><span>{eventDate}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Amount</span><span style=\"color:#22c55e;font-weight:700\">₪{amount}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Status</span><span style=\"color:#22c55e;font-weight:700\">✓ Paid</span></div>" +
                $"</div>" +
                $"<p>Thank you! See you at the event 🎉</p>";
            return Build("Payment received! 💳",
                $"Your payment of ₪{amount} for \"{eventTitle}\" has been confirmed.", body);
        }

        public static string PaymentManagerNotification(string managerName, string participantName, string participantEmail, string eventTitle, decimal amount)
        {
            var body =
                $"<p>Hi <strong>{managerName}</strong>,</p>" +
                $"<p>A participant has completed their payment for your event:</p>" +
                $"<div class=\"info-box\">" +
                $"<div class=\"info-row\"><span class=\"info-label\">Event</span><span>{eventTitle}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Participant</span><span>{participantName} ({participantEmail})</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Amount</span><span style=\"color:#22c55e;font-weight:700\">₪{amount}</span></div>" +
                $"</div>";
            return Build("Payment received 💰",
                $"{participantName} paid ₪{amount} for \"{eventTitle}\".", body);
        }

        public static string PollCreated(string participantName, string eventTitle, string pollName, int questionCount)
        {
            var body =
                $"<p>Hi <strong>{participantName}</strong>,</p>" +
                $"<p>A new poll has been created for your event. Your opinion matters!</p>" +
                $"<div class=\"info-box\">" +
                $"<div class=\"info-row\"><span class=\"info-label\">Event</span><span>{eventTitle}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Poll</span><span>{pollName}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Questions</span><span>{questionCount}</span></div>" +
                $"</div>" +
                $"<p>Log in to GatherUp to cast your vote.</p>";
            return Build("New poll available! 🗳️",
                $"A new poll has been created for \"{eventTitle}\".", body);
        }

        public static string EventDetailsChanged(string participantName, string eventTitle, string eventDate, string eventLocation)
        {
            var body =
                $"<p>Hi <strong>{participantName}</strong>,</p>" +
                $"<p>The organizer has updated the details for your upcoming event:</p>" +
                $"<div class=\"info-box\">" +
                $"<div class=\"info-row\"><span class=\"info-label\">Event</span><span>{eventTitle}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Date</span><span>{eventDate}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Location</span><span>{eventLocation}</span></div>" +
                $"</div>" +
                $"<p>Please make note of any changes. See you there! 🎉</p>";
            return Build("Event details updated 📝",
                $"The details for \"{eventTitle}\" have been updated.", body);
        }
        public static string PendingInvitation(string participantName, string eventTitle, string eventDate, string eventLocation)
        {
            var body =
                $"<p>Hi <strong>{participantName}</strong>,</p>" +
                $"<p>You've been invited to the following event. Please let the organizer know if you'll be attending.</p>" +
                $"<div class=\"info-box\">" +
                $"<div class=\"info-row\"><span class=\"info-label\">Event</span><span>{eventTitle}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Date</span><span>{eventDate}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Location</span><span>{eventLocation}</span></div>" +
                $"</div>" +
                $"<p>Please log in to GatherUp and confirm your attendance as soon as possible.</p>" +
                $"<p style=\"font-size:13px;color:#94a3b8\">If you have already responded, please ignore this reminder.</p>";
            return Build("Will you be attending?",
                $"Hello {participantName}, please confirm your attendance for \"{eventTitle}\".", body);
        }

        public static string PaymentReminder(string participantName, string eventTitle, string eventDate, string eventLocation)
        {
            var body =
                $"<p>Hi <strong>{participantName}</strong>,</p>" +
                $"<p>This is a friendly reminder that your payment for the following event is still <strong>pending</strong>.</p>" +
                $"<div class=\"info-box\">" +
                $"<div class=\"info-row\"><span class=\"info-label\">Event</span><span>{eventTitle}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Date</span><span>{eventDate}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Location</span><span>{eventLocation}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Bank</span><span>Bank 12 &bull; Branch 345 &bull; Account 678901</span></div>" +
                $"</div>" +
                $"<p>Please complete your payment as soon as possible so we can finalize the event arrangements.</p>" +
                $"<p style=\"font-size:13px;color:#94a3b8\">If you have already paid, please ignore this message.</p>";
            return Build("Payment reminder",
                $"Hello {participantName}, your payment for \"{eventTitle}\" is still pending.", body);
        }

        public static string HostInvitation(string hostName, string eventTitle, string eventDate, string eventLocation)
        {
            var body =
                $"<p>Hi <strong>{hostName}</strong>,</p>" +
                $"<p>Great news! You've been selected as the <strong>host</strong> for the following event. Your role is to provide the venue and welcome the guests.</p>" +
                $"<div class=\"info-box\">" +
                $"<div class=\"info-row\"><span class=\"info-label\">Event</span><span>{eventTitle}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Date</span><span>{eventDate}</span></div>" +
                $"<div class=\"info-row\"><span class=\"info-label\">Location</span><span>{eventLocation}</span></div>" +
                $"</div>" +
                $"<p>Please make sure everything is ready before the event date. The event manager will be in touch with more details.</p>" +
                $"<p>Thank you for hosting with <strong>GatherUp</strong>!</p>";
            return Build("You're invited to host an event!",
                $"Hello {hostName}, you have been selected as the host for \"{eventTitle}\".", body);
        }
    }
}
