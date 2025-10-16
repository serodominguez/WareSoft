using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Users user);
    }
}
