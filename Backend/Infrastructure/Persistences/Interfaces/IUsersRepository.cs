using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IUsersRepository : IGenericRepository<Users>
    {
        IQueryable<Users> ListUsers();
        Task<Users?> AccountByUserName(string userName);
        Task<bool> EditUser(Users user, bool? updatePassword);
    }
}
