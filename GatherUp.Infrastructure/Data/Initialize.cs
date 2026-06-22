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
            var odelia = new Participant
            {
                Id = 1,
                Name = "Odelia Nakacshe",
                Email = "odelia50872@gmail.com",
                PhoneNumber = "0583250876",
                Password = "210028221",
                Role = UserRole.Manager,
                MailingPreferences = MailingPreference.AttendanceConfirmation | MailingPreference.PaymentConfirmation | MailingPreference.PollResponses
            };

            var host1 = new Participant
            {
                Id = 2,
                Name = "Michal Levi",
                Email = "michal.levi@gmail.com",
                PhoneNumber = "052-7654321",
                Password = "123456789",
                Role = UserRole.Host
            };

            var avital = new Participant
            {
                Id = 3,
                Name = "Avital Cohen",
                Email = "avitalbc123@gmail.com",
                PhoneNumber = "0583260632",
                Password = "327584173",
                Role = UserRole.Participant,
                MailingPreferences = MailingPreference.AttendanceConfirmation | MailingPreference.PaymentConfirmation | MailingPreference.PollResponses
            };

            var noa = new Participant
            {
                Id = 4,
                Name = "Noa Cohen",
                Email = "noa.cohen@gmail.com",
                PhoneNumber = "050-1234567",
                Password = "111222333",
                Role = UserRole.Manager,
                MailingPreferences = MailingPreference.AttendanceConfirmation | MailingPreference.PaymentConfirmation | MailingPreference.PollResponses
            };

            var host2 = new Participant
            {
                Id = 5,
                Name = "Dana Israeli",
                Email = "dana@gmail.com",
                PhoneNumber = "050-9999999",
                Password = "444555666",
                Role = UserRole.Host
            };

            participantRepo.Add(odelia);
            participantRepo.Add(host1);
            participantRepo.Add(avital);
            participantRepo.Add(noa);
            participantRepo.Add(host2);

            // --- Event 1: Odelia is Manager ---
            var vendor1 = new VendorAllocation
            {
                Id = 1,
                Name = "Happy Catering",
                AmountOwed = 5000,
                HasReceipt = true
            };
            vendor1.Receipts.Add(new ReceiptDetails("REC-2026-001", 2000, DateTime.Now));

            var poll1 = new Poll { Id = 1, Name = "Initial Preferences", Description = "Date and location poll" };
            poll1.Questions.Add(new PollQuestion
            {
                Id = 101,
                QuestionText = "Which location do you prefer?",
                Options = new List<string> { "Jerusalem", "Tel Aviv", "Haifa" }
            });
            poll1.Questions[0].Responses.Add(new PollResponse { ParticipantId = odelia.Id, Response = "Jerusalem" });
            poll1.Questions[0].Responses.Add(new PollResponse { ParticipantId = avital.Id, Response = "Tel Aviv" });
            pollRepo.Add(poll1);

            var event1 = new Event
            {
                Id = 1,
                Title = "GatherUp Launch Hackathon",
                Date = new DateTime(2026, 06, 15),
                Location = "Jerusalem",
                EventManagerId = odelia.Id,
                EventHostId = host1.Id,
                ParticipantIds = new List<int> { odelia.Id, avital.Id },
                ParticipantData = new List<EventParticipantData>
                {
                    new EventParticipantData { ParticipantId = odelia.Id, IsAttending = true, HasPaid = true, AmountContributed = 150 },
                    new EventParticipantData { ParticipantId = avital.Id, IsAttending = null, HasPaid = false }
                },
                Vendors = new List<VendorAllocation> { vendor1 },
                PollIds = new List<int> { poll1.Id }
            };
            eventRepo.Add(event1);

            // --- Event 2: Odelia is Participant, Noa is Manager ---
            var vendor2 = new VendorAllocation
            {
                Id = 1,
                Name = "Sound System Co.",
                AmountOwed = 3000,
                HasReceipt = false
            };

            var poll2 = new Poll { Id = 2, Name = "Catering Poll", Description = "Food preferences" };
            poll2.Questions.Add(new PollQuestion
            {
                Id = 201,
                QuestionText = "Which main course do you prefer?",
                Options = new List<string> { "Meat", "Vegetarian", "Vegan" }
            });
            poll2.Questions[0].Responses.Add(new PollResponse { ParticipantId = noa.Id, Response = "Meat" });
            poll2.Questions[0].Responses.Add(new PollResponse { ParticipantId = odelia.Id, Response = "Vegetarian" });
            pollRepo.Add(poll2);

            var event2 = new Event
            {
                Id = 2,
                Title = "End of Year Celebration",
                Date = new DateTime(2026, 12, 31),
                Location = "Tel Aviv",
                EventManagerId = noa.Id,
                EventHostId = host2.Id,
                ParticipantIds = new List<int> { noa.Id, odelia.Id, avital.Id },
                ParticipantData = new List<EventParticipantData>
                {
                    new EventParticipantData { ParticipantId = noa.Id, IsAttending = true, HasPaid = true, AmountContributed = 200 },
                    new EventParticipantData { ParticipantId = odelia.Id, IsAttending = null, HasPaid = false },
                    new EventParticipantData { ParticipantId = avital.Id, IsAttending = null, HasPaid = false }
                },
                Vendors = new List<VendorAllocation> { vendor2 },
                PollIds = new List<int> { poll2.Id }
            };
            eventRepo.Add(event2);
        }
    }
}
