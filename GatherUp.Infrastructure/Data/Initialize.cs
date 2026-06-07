using GatherUp.core.DO;
using GatherUp.core.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Infrastructure.Data
{
    public static class InitializeData
    {
        public static void Initialize(
            IRepository<Event> eventRepo,
            IRepository<Participant> participantRepo,
            IRepository<EventManager> managerRepo,
            IRepository<EventHost> hostRepo,
            IRepository<Poll> pollRepo)
        {
            var manager = new EventManager
            {
                Id = 1,
                Name = "Noa Cohen",
                Email = "noa.cohen@gmail.com",
                PhoneNumber = "050-1234567" 
            }; 
            var host = new EventHost
            { 
                Id = 2,
                Name = "Michal Levi",
                Email = "michal.levi@gmail.com",
                PhoneNumber = "052-7654321"
            };

            managerRepo.Add(manager);
            hostRepo.Add(host);

            var p1 = new Participant { Id = 10, Name = "Odelia Nakacshe", Email = "odelia50872@gmail.com", IsAttending = true, HasPaid = true, AmountContributed = 150, PhoneNumber = "0583250876" };
            var p2 = new Participant { Id = 11, Name = "Avital Cohen", Email = "avitalbc123@gmail.com", IsAttending = null, HasPaid = false, AmountContributed = 0 , PhoneNumber = "0583260632" };

            participantRepo.Add(p1);
            participantRepo.Add(p2);

            var vendor = new VendorAllocation
            {
                Id = 1,
                Name = "Happy Catering",
                AmountOwed = 5000,
                HasReceipt = true
            };
            vendor.Receipts.Add(new ReceiptDetails("REC-2026-001", 2000, DateTime.Now));

            var poll1 = new Poll { Id = 1, Name = "העדפות ראשוניות", Description = "סקר קביעת תאריך ומיקום" };
            poll1.Questions.Add(new PollQuestion
            {
                Id = 101,
                QuestionText = "איזה מיקום מועדף עליך?",
                Options = new List<string> { "ירושלים", "תל אביב", "חיפה" }
            });
            var poll2 = new Poll { Id = 2, Name = "סקר המשך", Description = "בחירת מנות קייטרינג" };
            poll2.Questions.Add(new PollQuestion
            {
                Id = 201,
                QuestionText = "מהי המנה העיקרית המועדפת עליך?",
                Options = new List<string> { "בשרי", "צמחוני", "טבעוני" }
            });

            pollRepo.Add(poll1);
            pollRepo.Add(poll2);

            var newEvent = new Event
            {
                Id = 1,
                Title = "GatherUp Launch Hackathon",
                Date = new DateTime(2026, 06, 15),
                Location = "Jerusalem",
                EventManagerId = manager.Id,
                EventHostId = host.Id,
                ParticipantIds = new List<int> { p1.Id, p2.Id },
                Vendors = new List<VendorAllocation> { vendor },
                PollIds = new List<int> { poll1.Id, poll2.Id }
            };

            eventRepo.Add(newEvent);
        }
    }
}