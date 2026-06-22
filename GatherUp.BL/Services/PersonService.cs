using GatherUp.core.DO;
using GatherUp.core.Exceptions;
using GatherUp.core.interfaces;

namespace GatherUp.BL.Services
{
    public class PersonService : IPersonService
    {
        private readonly IRepository<Participant> _participantRepo;

        public PersonService(IRepository<Participant> participantRepo)
        {
            _participantRepo = participantRepo;
        }

        public Participant GetById(int id) =>
            _participantRepo.GetById(id) ?? throw new EntityNotFoundException("Participant", id);

        public Participant UpdateParticipant(int id, string name, string phone)
        {
            var participant = _participantRepo.GetById(id)
                ?? throw new EntityNotFoundException("Participant", id);

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidEventDataException("Name cannot be empty.");

            participant.Name = name;
            participant.PhoneNumber = phone;
            _participantRepo.Update(participant);
            return participant;
        }
    }
}
