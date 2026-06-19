using GatherUp.BL.Services;
using GatherUp.core.DO;
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
            Console.WriteLine("=== Starting GatherUp System Test ===");

            string xmlFolder = Path.Combine(AppContext.BaseDirectory, "XML");
            if (Directory.Exists(xmlFolder)) Directory.Delete(xmlFolder, true);
            Directory.CreateDirectory(xmlFolder);

            // --- Infrastructure ---
            var eventRepo = new XmlRepository<Event>(xmlFolder);
            var participantRepo = new XmlRepository<Participant>(xmlFolder);
            var pollRepo = new XmlRepository<Poll>(xmlFolder);

            InitializeData.Initialize(eventRepo, participantRepo, pollRepo);
            Console.WriteLine("-> Data initialized.");

            // --- BL with dependency injection ---
            var mailService = new MailService(Path.Combine(AppContext.BaseDirectory, "MailLog.txt"));
            var eventService = new EventService(eventRepo, participantRepo);
            var authService = new AuthService(participantRepo);
            var participantService = new ParticipantService(participantRepo, eventRepo, mailService);
            var financialService = new FinancialService(participantRepo, eventRepo, mailService);
            var pollService = new PollService(pollRepo, eventRepo);
            var vendorService = new VendorService(eventRepo);

            // --- Event listeners ---
            // ParticipantService: manager gets email on attendance confirmation
            participantService.OnAttendanceConfirmed += (eventId, participantId) =>
            {
                var ev = eventRepo.GetById(eventId);
                var manager = participantRepo.GetAll().FirstOrDefault(p => p.Id == ev.EventManagerId);
                var participant = participantRepo.GetById(participantId);
                if (manager != null && participant != null)
                    mailService.SendEmail(manager.Email,
                        $"Attendance Confirmed - {ev.Title}",
                        $"Participant {participant.Name} confirmed attendance.");
            };

            // FinancialService: manager gets email on payment
            financialService.OnPaymentReceived += (eventId, participantId) =>
            {
                var ev = eventRepo.GetById(eventId);
                var manager = participantRepo.GetAll().FirstOrDefault(p => p.Id == ev.EventManagerId);
                var participant = participantRepo.GetById(participantId);
                if (manager != null && participant != null)
                    mailService.SendEmail(manager.Email,
                        $"Payment Received - {ev.Title}",
                        $"Participant {participant.Name} completed payment.");
            };

            // PollService: participants get email on new poll
            pollService.OnPollCreated += (pollId) =>
            {
                var poll = pollRepo.GetById(pollId);
                var ev = eventRepo.GetAll().FirstOrDefault(e => e.PollIds.Contains(pollId));
                if (ev == null) return;
                participantRepo.GetAll()
                    .Where(p => ev.ParticipantIds.Contains(p.Id) && p.MailingPreferences != MailingPreference.None)
                    .ToList()
                    .ForEach(p => mailService.SendEmail(p.Email,
                        $"New Poll: {poll?.Name}",
                        $"Hello {p.Name}, a new poll was created for {ev.Title}."));
            };

            // EventService: participants get email on event update
            eventService.OnEventDetailsChanged += (eventId) =>
            {
                var ev = eventRepo.GetById(eventId);
                participantRepo.GetAll()
                    .Where(p => ev.ParticipantIds.Contains(p.Id) && p.MailingPreferences != MailingPreference.None)
                    .ToList()
                    .ForEach(p => mailService.SendEmail(p.Email,
                        $"Event Updated: {ev.Title}",
                        $"Hello {p.Name}, details for {ev.Title} have been updated."));
            };

            // [Screen: Login]
            Console.WriteLine("\n=== [Screen: Login] Manager login ===");
            var loggedIn = authService.Login("noa.cohen@gmail.com", "050-1234567");
            Console.WriteLine($"-> Logged in: {loggedIn.Name} | Role: {loggedIn.Role}");

            // [Screen: Register]
            Console.WriteLine("\n=== [Screen: Register] New participant ===");
            var newUser = authService.Register("Dana Israeli", "dana@gmail.com", "050-9999999", "050-9999999");
            Console.WriteLine($"-> Registered: {newUser.Name} (Id={newUser.Id})");

            // [Screen: Event] Confirm attendance
            Console.WriteLine("\n=== [Screen: Event] Confirm Attendance for participant 10 ===");
            participantService.ConfirmAttendance(10, true);

            // [Screen: Management] Send attendance reminders
            Console.WriteLine("\n=== [Screen: Management] Send Attendance Reminders ===");
            participantService.SendPendingInvitations(1);

            // [Screen: Management] Confirm payment
            Console.WriteLine("\n=== [Screen: Management] Confirm Payment for participant 10 ===");
            financialService.RegisterPayment(10, 150);

            // [Screen: Management] Send payment reminders
            Console.WriteLine("\n=== [Screen: Management] Send Payment Reminders ===");
            financialService.SendPaymentReminders(1);

            // [Screen: Vendor Management] Add vendor and set amount
            Console.WriteLine("\n=== [Screen: Vendor Management] Add vendor ===");
            var vendor = vendorService.AddVendor(1, "Sound System Co.", 3000);
            Console.WriteLine($"-> Vendor added: {vendor.Name}");

            Console.WriteLine("\n=== [Screen: Vendor Management] Set vendor amount ===");
            financialService.SetVendorAmount(1, vendor.Id, 2500);

            // [Screen: Financial Report]
            Console.WriteLine("\n=== [Screen: Financial Report] Account Summary ===");
            var summary = financialService.GetFinancialSummary(1);
            Console.WriteLine($"Income: {summary.TotalIncome} | Outgoing: {summary.TotalOutgoing} | Balance: {summary.Balance}");
            summary.PaidParticipants.ToList().ForEach(p => Console.WriteLine($"  - {p.Name}: {p.AmountContributed}"));

            // [Screen: Management] Create poll
            Console.WriteLine("\n=== [Screen: Management] Create New Poll ===");
            var newPoll = pollService.CreatePoll(1, "Date Poll", new List<(string, List<string>)>
            {
                ("Which date works?", new List<string> { "Aug 1", "Aug 15", "Sep 1" })
            });
            Console.WriteLine($"-> Poll created: {newPoll.Name} (Id={newPoll.Id})");

            // [Screen: Participant] Vote
            Console.WriteLine("\n=== [Screen: Participant] Vote in poll ===");
            pollService.SubmitVote(newPoll.Id, newPoll.Questions[0].Id, 10, "Aug 1");
            pollService.SubmitVote(newPoll.Id, newPoll.Questions[0].Id, 11, "Aug 1");

            // [Screen: Poll Results]
            Console.WriteLine("\n=== [Screen: Poll Results] ===");
            pollService.GetPollResults(newPoll.Id).ToList().ForEach(r =>
            {
                Console.WriteLine($"Question: {r.QuestionText}");
                r.Results.ToList().ForEach(o => Console.WriteLine($"  {o.Option}: {o.Votes} votes ({o.Percentage}%)"));
            });

            Console.WriteLine("\n=== Test Completed Successfully! ===");
        }
    }
}
