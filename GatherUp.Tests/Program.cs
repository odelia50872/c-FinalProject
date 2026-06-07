using GatherUp.core.DO;
using GatherUp.Infrastructure.Data;
using GatherUp.Infrastructure.Memory;
using GatherUp.Infrastructure.Repositories;
using GatherUp.core.interfaces;

namespace GatherUp.Tests
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Starting GatherUp System Test ===");

            string xmlFolder = Path.Combine(AppContext.BaseDirectory, "XML");
            string receiptsFolder = Path.Combine(AppContext.BaseDirectory, "Receipts");

            if (Directory.Exists(xmlFolder))
                Directory.Delete(xmlFolder, true);
            Directory.CreateDirectory(xmlFolder);
            Directory.CreateDirectory(receiptsFolder);

            RunWithMemory();
            RunWithXml(xmlFolder, receiptsFolder);
        }

        static void RunWithMemory()
        {
            Console.WriteLine("\n=== Memory Repository ===");
            var eventRepo = new MemoryRepository<Event>();
            var participantRepo = new MemoryRepository<Participant>();
            var managerRepo = new MemoryRepository<EventManager>();
            var hostRepo = new MemoryRepository<EventHost>();
            var pollRepo = new MemoryRepository<Poll>();

            InitializeData.Initialize(eventRepo, participantRepo, managerRepo, hostRepo, pollRepo);
            Console.WriteLine("-> Data initialized in memory.");
        }

        static void RunWithXml(string xmlFolder, string receiptsFolder)
        {
            Console.WriteLine("\n=== XML Repository ===");

            var eventRepo = new XmlRepository<Event>(xmlFolder);
            var participantRepo = new XmlRepository<Participant>(xmlFolder);
            var managerRepo = new XmlRepository<EventManager>(xmlFolder);
            var hostRepo = new XmlRepository<EventHost>(xmlFolder);
            var pollRepo = new XmlRepository<Poll>(xmlFolder);
            var receiptRepo = new ReceiptRepository(xmlFolder, receiptsFolder);

            InitializeData.Initialize(eventRepo, participantRepo, managerRepo, hostRepo, pollRepo);
            Console.WriteLine("-> Data initialized to XML files.");

            // הוספת 3 משתתפים נוספים
            Console.WriteLine("\n-> Adding 3 new participants...");
            participantRepo.Add(new Participant { Id = 101, Name = "Adi Levi", Email = "adi@gmail.com", PhoneNumber = "050-1112223", IsAttending = true });
            participantRepo.Add(new Participant { Id = 102, Name = "Roni Cohen", Email = "roni@gmail.com", PhoneNumber = "052-4445556", IsAttending = false });
            participantRepo.Add(new Participant { Id = 103, Name = "Shira Avni", Email = "shira@gmail.com", PhoneNumber = "054-7778889", IsAttending = null });

            // הוספת שאלה לסקר
            Console.WriteLine("-> Adding question to poll...");
            var poll = pollRepo.GetById(1);
            poll.Questions.Add(new PollQuestion { Id = 102, QuestionText = "באיזה שעה מועדפת?", Options = new List<string> { "בוקר", "צהריים", "ערב" } });
            pollRepo.Update(poll);

            // שינוי תשובה בסקר
            Console.WriteLine("-> Updating poll response...");
            poll.Questions[0].ParticipantResponses[10] = "תל אביב";
            pollRepo.Update(poll);

            // הדפסת כל המשתתפים
            Console.WriteLine("\n=== All Participants ===");
            foreach (var p in participantRepo.GetAll())
                Console.WriteLine($"[{p.Id}] {p.Name} | {p.Email} | {p.PhoneNumber}");

            // הוספת קבלה עם קובץ
            Console.WriteLine("\n-> Adding receipt...");
            string testFile = Path.Combine(AppContext.BaseDirectory, "test_receipt.txt");
            File.WriteAllText(testFile, "קבלה לדוגמה");
            receiptRepo.AddReceipt(
                new ReceiptDetails("REC-2026-002", 1500, DateTime.Now),
                testFile
            );
            Console.WriteLine($"-> Receipt added. File copied to: {receiptsFolder}");

            Console.WriteLine("\n=== Test Completed Successfully! ===");
        }
    }
}