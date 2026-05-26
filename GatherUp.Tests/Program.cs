using GatherUp.core.DO;
using GatherUp.Infrastructure.Data;
using GatherUp.Infrastructure.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Tests
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== תחילת בדיקת מערכת GatherUp ===");

            var eventRepo = new MemoryRepository<Event>();
            var participantRepo = new MemoryRepository<Participant>();
            var managerRepo = new MemoryRepository<EventManager>();
            var hostRepo = new MemoryRepository<EventHost>();
            var pollRepo = new MemoryRepository<Poll>();

            InitializeData.Initialize(eventRepo, participantRepo, managerRepo, hostRepo, pollRepo);
            Console.WriteLine("-> נתוני הבדיקה ההתחלתיים אותחלו בהצלחה בזיכרון.");

            Console.WriteLine("\n-> מוסיף 3 משתתפים חדשים...");

            var newP1 = new Participant { Id = 101, Name = "Adi Levi", Email = "adi@example.com", PhoneNumber = "050-1112223", IsAttending = true };
            var newP2 = new Participant { Id = 102, Name = "Roni Cohen", Email = "roni@example.com", PhoneNumber = "052-4445556", IsAttending = false };
            var newP3 = new Participant { Id = 103, Name = "Shira Avni", Email = "shira@example.com", PhoneNumber = "054-7778889", IsAttending = null };

            participantRepo.Add(newP1);
            participantRepo.Add(newP2);
            participantRepo.Add(newP3);

            Console.WriteLine("\n-> שולף את משתתף מספר 102 מהרשימה...");
            var searchedParticipant = participantRepo.GetById(102);
            if (searchedParticipant != null)
            {
                Console.WriteLine($"נמצא משתתף: {searchedParticipant.Name} | טלפון: {searchedParticipant.PhoneNumber}");
            }

            Console.WriteLine("\n=== רשימת כל המשתתפים במערכת ===");
            var allParticipants = participantRepo.GetAll();
            foreach (var participant in allParticipants)
            {
                Console.WriteLine($"[ID: {participant.Id}] שם: {participant.Name} | טלפון: {participant.PhoneNumber} | מייל: {participant.Email}");
            }

            Console.WriteLine("\n=== הבדיקה הסתיימה בהצלחה! ===");
        }
    }
}
