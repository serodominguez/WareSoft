using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IUsersRepository
    {
        Task<BaseEntityResponse<Users>> ListUsers(BaseFiltersRequest filters);
        Task<IEnumerable<Users>> ListSelectUsers();
        Task<Users> UserById(int userId);
        Task<bool> RegisterUser(Users users);
        Task<bool> EditUser(Users users);
        Task<bool> EnableUser(int userId);
        Task<bool> DisableUser(int userId);
        Task<bool> RemoveUser(int userId);
    }
}
