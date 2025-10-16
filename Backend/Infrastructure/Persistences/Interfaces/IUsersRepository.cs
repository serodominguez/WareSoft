using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IUsersRepository
    {
        Task<BaseEntityResponse<Users>> ListUsers(BaseFiltersRequest filters);
        Task<Users> UserById(int userId);
        Task<bool> RegisterUser(Users user, string password);
        Task<bool> EditUser(Users user, bool updatePassword, string password);
        Task<bool> EnableUser(int userId);
        Task<bool> DisableUser(int userId);
        Task<bool> RemoveUser(int userId);
    }
}
