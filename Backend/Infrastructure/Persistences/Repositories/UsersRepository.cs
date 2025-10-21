using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class UsersRepository : GenericRepository<Users>, IUsersRepository
    {
        private readonly DbContextSystem _context;

        public UsersRepository(DbContextSystem context) : base(context)
        {
            _context = context;
        }

        public async Task<Users?> AccountByUserName(string userName)
        {
            var account = await _context.Users
                            .Where(u => u.AUDIT_DELETE_USER == null &&
                                        u.AUDIT_DELETE_DATE == null &&
                                        u.STATE == true &&
                                        u.USER_NAME == userName)
                            .Include(u => u.Roles)
                            .Include(u => u.Stores)
                            .AsNoTracking()
                            .FirstOrDefaultAsync();

            return account;
        }

        public async Task<bool> EditUser(Users user, bool? updatePassword)
        {
            user.AUDIT_UPDATE_USER = 1;
            user.AUDIT_UPDATE_DATE = DateTime.Now;

            if (updatePassword == true)
            {
                _context.Update(user);
                _context.Entry(user).Property(x => x.STATE).IsModified = false;
                _context.Entry(user).Property(x => x.AUDIT_CREATE_USER).IsModified = false;
                _context.Entry(user).Property(x => x.AUDIT_CREATE_DATE).IsModified = false;
            }
            else
            {
                _context.Update(user);
                _context.Entry(user).Property(x => x.PASSWORD_HASH).IsModified = false;
                _context.Entry(user).Property(x => x.PASSWORD_SALT).IsModified = false;
                _context.Entry(user).Property(x => x.STATE).IsModified = false;
                _context.Entry(user).Property(x => x.AUDIT_CREATE_USER).IsModified = false;
                _context.Entry(user).Property(x => x.AUDIT_CREATE_DATE).IsModified = false;
            }

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }
    }
}
