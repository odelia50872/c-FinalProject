using GatherUp.core.DO;

namespace GatherUp.core.interfaces
{
    public interface IAuthService
    {
        Participant? Login(string email, string password);
        Participant Register(string name, string email, string phone, string password);
    }
}
