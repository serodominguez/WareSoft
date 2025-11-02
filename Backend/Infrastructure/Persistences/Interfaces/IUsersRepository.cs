using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IUsersRepository : IGenericRepository<Users>
    {
        IQueryable<Users> GetUsersQueryable();
        Task<Users?> AccountByUserNameAsync(string userName);
        Task<bool> EditUserAsync(int authenticatedUserId, Users user, bool? updatePassword);
    }
}
