using GatherUp.core.DO;
using GatherUp.core.interfaces;

namespace GatherUp.Infrastructure.Data
{
    public static class InitializeData
    {
        public static void Initialize(
            IRepository<Event> eventRepo,
            IRepository<Participant> participantRepo,
            IRepository<Poll> pollRepo)
        {
            var manager = new Participant
            {
                Id = 1,
                Name = "Noa Cohen",
                Email = "noa.cohen@gmail.com",
                PhoneNumber = "050-1234567",
                Role = UserRole.Manager
            };

            var host = new Participant
            {
                Id = 2,
                Name = "Michal Levi",
                Email = "michal.levi@gmail.com",
                PhoneNumber = "052-7654321",
                Role = UserRole.Host
            };

            var p1 = new Participant
            {
                Id = 10,
                Name = "Odelia Nakacshe",
                Email = "odelia50872@gmail.com",
                PhoneNumber = "0583250876",
                IsAttending = true,
                HasPaid = true,
                AmountContributed = 150,
                Role = UserRole.Participant
            };

            var p2 = new Participant
            {
                Id = 11,
                Name = "Avital Cohen",
                Email = "avitalbc123@gmail.com",
                PhoneNumber = "0583260632",
                IsAttending = null,
                HasPaid = false,
                Role = UserRole.Participant
            };

            participantRepo.Add(manager);
            participantRepo.Add(host);
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

            var poll1 = new Poll { Id = 1, Name = "Initial Preferences", Description = "Date and location poll" };
            poll1.Questions.Add(new PollQuestion
            {
                Id = 101,
                QuestionText = "Which location do you prefer?",
                Options = new List<string> { "Jerusalem", "Tel Aviv", "Haifa" }
            });

            var poll2 = new Poll { Id = 2, Name = "Catering Poll", Description = "Food preferences" };
            poll2.Questions.Add(new PollQuestion
            {
                Id = 201,
                QuestionText = "Which main course do you prefer?",
                Options = new List<string> { "Meat", "Vegetarian", "Vegan" }
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
