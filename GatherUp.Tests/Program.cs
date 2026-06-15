using GatherUp.BL.Services;
using GatherUp.core.DO;
using GatherUp.core.interfaces;
using GatherUp.Infrastructure.Data;
using GatherUp.Infrastructure.Repositories;
using GatherUp.Infrastructure.Services;

namespace GatherUp.Tests
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", "/c chcp 65001") { CreateNoWindow = true })?.WaitForExit();
            Console.WriteLine("=== Starting GatherUp System Test ===");

            string xmlFolder = Path.Combine(AppContext.BaseDirectory, "XML");
            string receiptsFolder = Path.Combine(AppContext.BaseDirectory, "Receipts");

            if (Directory.Exists(xmlFolder)) Directory.Delete(xmlFolder, true);
            Directory.CreateDirectory(xmlFolder);
            Directory.CreateDirectory(receiptsFolder);

            var eventRepo = new XmlRepository<Event>(xmlFolder);
            var participantRepo = new XmlRepository<Participant>(xmlFolder);
            var managerRepo = new XmlRepository<EventManager>(xmlFolder);
            var hostRepo = new XmlRepository<EventHost>(xmlFolder);
            var pollRepo = new XmlRepository<Poll>(xmlFolder);

            InitializeData.Initialize(eventRepo, participantRepo, managerRepo, hostRepo, pollRepo);
            Console.WriteLine("-> Data initialized to XML files.");

            var mailService = new MailService(Path.Combine(AppContext.BaseDirectory, "MailLog.txt"));
            var eventService = new EventService(eventRepo, participantRepo);
            var participantService = new ParticipantService(participantRepo, eventRepo, mailService, eventService);
            var financialService = new FinancialService(participantRepo, eventRepo, mailService, eventService);
            var pollService = new PollService(pollRepo, eventRepo, mailService, eventService);

            eventService.OnAttendanceConfirmed += (eventId, participantId) =>
            {
                var ev = eventRepo.GetById(eventId);
                var manager = managerRepo.GetById(ev.EventManagerId);
                var participant = participantRepo.GetById(participantId);
                if (manager != null && participant != null)
                    mailService.SendEmail(manager.Email,
                        $"אישור הגעה - {ev.Title}",
                        $"המשתתף {participant.Name} אישר הגעה לאירוע.");
            };

            eventService.OnPaymentReceived += (eventId, participantId) =>
            {
                var ev = eventRepo.GetById(eventId);
                var manager = managerRepo.GetById(ev.EventManagerId);
                var participant = participantRepo.GetById(participantId);
                if (manager != null && participant != null)
                    mailService.SendEmail(manager.Email,
                        $"תשלום התקבל - {ev.Title}",
                        $"המשתתף {participant.Name} ביצע תשלום.");
            };

            eventService.OnPollCreated += (pollId) =>
            {
                var poll = pollRepo.GetById(pollId);
                var ev = eventRepo.GetAll().FirstOrDefault(e => e.PollIds.Contains(pollId));
                if (ev == null) return;
                participantRepo.GetAll()
                    .Where(p => ev.ParticipantIds.Contains(p.Id) &&
                                p.MailingPreferences != MailingPreference.None)
                    .ToList()
                    .ForEach(p => mailService.SendEmail(p.Email,
                        $"סקר חדש נוצר: {poll?.Name}",
                        $"שלום {p.Name}, נוצר סקר חדש באירוע {ev.Title}. נא למלא."));
            };

            eventService.OnEventDetailsChanged += (eventId) =>
            {
                var ev = eventRepo.GetById(eventId);
                participantRepo.GetAll()
                    .Where(p => ev.ParticipantIds.Contains(p.Id) &&
                                p.MailingPreferences != MailingPreference.None)
                    .ToList()
                    .ForEach(p => mailService.SendEmail(p.Email,
                        $"עדכון פרטי אירוע: {ev.Title}",
                        $"שלום {p.Name}, פרטי האירוע {ev.Title} עודכנו."));
            };

            Console.WriteLine("\n=== [מסך: אירוע] לחיצה על 'אשר הגעה' עבור משתתף 10 ===");
            participantService.ConfirmAttendance(10, true);

            Console.WriteLine("\n=== [מסך: ניהול] לחיצה על 'שלח תזכורות לאישור הגעה' ===");
            participantService.SendPendingInvitations(1);

            Console.WriteLine("\n=== [מסך: ניהול] לחיצה על 'אשר תשלום' עבור משתתף 10 ===");
            financialService.RegisterPayment(10, 150);

            Console.WriteLine("\n=== [מסך: ניהול] לחיצה על 'שלח תזכורות תשלום' ===");
            financialService.SendPaymentReminders(1);

            Console.WriteLine("\n=== [מסך: ניהול] לחיצה על 'הוסף חוב לספק 1' ===");
            financialService.AddVendorDebt(1, 1, 500);

            Console.WriteLine("\n=== [מסך: דוח פיננסי] הצגת סיכום מצב חשבון ===");
            var summary = financialService.GetFinancialSummary(1);
            Console.WriteLine($"הכנסות: {summary.TotalIncome} | הוצאות: {summary.TotalOutgoing} | מאזן: {summary.Balance}");
            Console.WriteLine("משתתפים ששילמו:");
            summary.PaidParticipants.ToList().ForEach(p => Console.WriteLine($"  - {p.Name}: {p.AmountContributed}"));

            Console.WriteLine("\n=== [מסך: דוח] רשימת קבלות ממוינות ===");
            financialService.GetAllReceiptsSorted(1).ToList()
                .ForEach(r => Console.WriteLine($"  קבלה {r.ReceiptNumber}: {r.Amount}"));

            Console.WriteLine("\n=== [מסך: ניהול] לחיצה על 'צור סקר חדש' ===");
            var newPoll = pollService.CreatePoll(1, "סקר תאריך", new List<(string, List<string>)>
            {
                ("איזה תאריך מתאים?", new List<string> { "1/8", "15/8", "1/9" })
            });
            Console.WriteLine($"-> סקר נוצר: {newPoll.Name} (Id={newPoll.Id})");

            Console.WriteLine("\n=== [מסך: משתתף] לחיצה על 'הצבע' בסקר ===");
            pollService.SubmitVote(newPoll.Id, newPoll.Questions[0].Id, 10, "1/8");
            pollService.SubmitVote(newPoll.Id, newPoll.Questions[0].Id, 11, "1/8");

            Console.WriteLine("\n=== [מסך: תוצאות סקר] הצגת תוצאות ===");
            pollService.GetPollResults(newPoll.Id).ToList().ForEach(r =>
            {
                Console.WriteLine($"שאלה: {r.QuestionText}");
                r.Results.ToList().ForEach(o => Console.WriteLine($"  {o.Option}: {o.Votes} קולות ({o.Percentage}%)"));
            });

            Console.WriteLine("\n=== Test Completed Successfully! ===");
        }
    }
}
