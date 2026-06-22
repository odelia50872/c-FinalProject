using GatherUp.core.DO;

namespace GatherUp.core.interfaces
{
    public interface IPersonService
    {
        Participant GetById(int id);
        Participant UpdateParticipant(int id, string name, string phone);
    }
}
