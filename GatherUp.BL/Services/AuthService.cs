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

        // Login screen - email is username, phone number is password
        public Participant Login(string email, string password)
        {
            var user = _participantRepo.GetAll()
                .FirstOrDefault(p => p.Email == email && p.PhoneNumber == password)
                ?? throw new UnauthorizedException("Invalid email or password.");

            return user;
        }

        // Register screen - creates a new participant
        public Participant Register(string name, string email, string phone, string password)
        {
            if (_participantRepo.GetAll().Any(p => p.Email == email))
                throw new ValidationException($"A user with email {email} already exists.");

            var newId = _participantRepo.GetAll().Any()
                ? _participantRepo.GetAll().Max(p => p.Id) + 1
                : 1;

            var participant = new Participant
            {
                Id = newId,
                Name = name,
                Email = email,
                PhoneNumber = phone,
                Role = UserRole.Participant
            };

            _participantRepo.Add(participant);
            return participant;
        }
    }
}
