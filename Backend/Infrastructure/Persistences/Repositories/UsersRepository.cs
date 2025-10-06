using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class UsersRepository : GenericRepository<Users>, IUsersRepository
    {
        private readonly DbContextSystem _context;

        public UsersRepository(DbContextSystem context)
        {
            _context = context;
        }

        public Task<BaseEntityResponse<Users>> ListUsers(BaseFiltersRequest filters)
        {
            //var response = new BaseEntityResponse<Users>();
            //var users = (from u in _context.Users
            //            where u.AUDIT_DELETE_USER == null && u.AUDIT_DELETE_DATE == null
            //            select u).AsNoTracking().AsQueryable();

            throw new NotImplementedException();
        }

        public Task<IEnumerable<Users>> ListSelectUsers()
        {
            throw new NotImplementedException();
        }
        public Task<Users> UserById(int userId)
        {
            throw new NotImplementedException();
        }
        public Task<bool> RegisterUser(Users users)
        {
            throw new NotImplementedException();
        }
        public Task<bool> EditUser(Users users)
        {
            throw new NotImplementedException();
        }
        public Task<bool> EnableUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DisableUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveUser(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
