using GatherUp.core.DO;
using GatherUp.core.Exceptions;
using GatherUp.core.interfaces;

namespace GatherUp.BL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<Participant> _participantRepo;

        public AuthService(IRepository<Participant> participantRepo)
        {
            _participantRepo = participantRepo;
        }

        // Login: email = מייל, password = תעודת זהות (נשמרת ב-Password)
        public Participant? Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            return _participantRepo.GetAll()
                .FirstOrDefault(p =>
                    p.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                    p.Password == password);
        }

        // Register: password = תעודת זהות
        public Participant Register(string name, string email, string phone, string password)
        {
            if (_participantRepo.GetAll().Any(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                throw new DuplicateUserException(email);

            if (string.IsNullOrWhiteSpace(password) || !password.All(char.IsDigit) || password.Length != 9)
                throw new InvalidEventDataException("Password must be a 9-digit ID number.");

            var newId = _participantRepo.GetAll().Any()
                ? _participantRepo.GetAll().Max(p => p.Id) + 1
                : 1;

            var participant = new Participant
            {
                Id = newId,
                Name = name,
                Email = email,
                PhoneNumber = phone,
                Password = password,
                Role = UserRole.Participant,
                MailingPreferences = MailingPreference.AttendanceConfirmation | MailingPreference.PaymentConfirmation | MailingPreference.PollResponses
            };

            _participantRepo.Add(participant);
            return participant;
        }
    }
}
