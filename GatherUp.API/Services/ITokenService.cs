using GatherUp.core.DO;

namespace GatherUp.API.Services
{
    public interface ITokenService
    {
        string GenerateToken(Participant user);
    }
}
