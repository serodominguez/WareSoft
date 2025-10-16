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
        private readonly IPasswordHasher _passwordHasher;

        public UsersRepository(DbContextSystem context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<BaseEntityResponse<Users>> ListUsers(BaseFiltersRequest filters)
        {
            var response = new BaseEntityResponse<Users>();
            var users = _context.Users
                            .Where(u => u.AUDIT_DELETE_USER == null && u.AUDIT_DELETE_DATE == null)
                            .Include(u => u.Roles)
                            .Include(u => u.Stores)
                            .AsNoTracking().AsQueryable();

            if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
            {
                switch (filters.NumberFilter)
                {
                    case 1:
                        users = users.Where(x => x.USER_NAME!.Contains(filters.TextFilter));
                        break;
                    case 2:
                        users = users.Where(x => x.NAMES!.Contains(filters.TextFilter));
                        break;
                    case 3:
                        users = users.Where(x => x.LAST_NAMES!.Contains(filters.TextFilter));
                        break;
                    case 4:
                        users = users.Where(x => x.Stores!.STORE_NAME!.Contains(filters.TextFilter));
                        break;
                    case 5:
                        users = users.Where(x => x.Roles!.ROLE_NAME!.Contains(filters.TextFilter));
                        break;
                }
            }

            if (filters.StateFilter is not null)
            {
                var stateValue = Convert.ToBoolean(filters.StateFilter);
                users = users.Where(x => x.STATE == stateValue);
            }

            if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
            {
                var startDate = Convert.ToDateTime(filters.StartDate).Date;
                var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);
                users = users.Where(x => x.AUDIT_CREATE_DATE >= startDate && x.AUDIT_CREATE_DATE <= endDate);
            }

            if (filters.Sort is null) filters.Sort = "PK_USERS";

            response.TotalRecords = await users.CountAsync();
            response.Items = await Ordering(filters, users, !(bool)filters.Download!).ToListAsync();

            return response;
        }

        public async Task<Users> UserById(int userId)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.PK_USER.Equals(userId));
            return user!;
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

        public async Task<bool> RegisterUser(Users user, string password)
        {
            _passwordHasher.GeneratePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PASSWORD_HASH = passwordHash;
            user.PASSWORD_SALT = passwordSalt;
            user.AUDIT_CREATE_USER = 1;
            user.AUDIT_CREATE_DATE = DateTime.Now;
            user.STATE = true;

            await _context.AddAsync(user);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> EditUser(Users user, bool updatePassword, string password)
        {
            user.AUDIT_UPDATE_USER = 1;
            user.AUDIT_UPDATE_DATE = DateTime.Now;

            if (updatePassword == true)
            {
                _passwordHasher.GeneratePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PASSWORD_HASH = passwordHash;
                user.PASSWORD_SALT = passwordSalt;
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

        public async Task<bool> EnableUser(int userId)
        {
            var user = await _context.Users.AsNoTracking().SingleOrDefaultAsync(x => x.PK_USER.Equals(userId));

            user!.AUDIT_UPDATE_USER = 1;
            user.AUDIT_UPDATE_DATE = DateTime.Now;
            user.STATE = true;
            _context.Update(user);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> DisableUser(int userId)
        {
            var user = await _context.Users.AsNoTracking().SingleOrDefaultAsync(x => x.PK_USER.Equals(userId));

            user!.AUDIT_UPDATE_USER = 1;
            user.AUDIT_UPDATE_DATE = DateTime.Now;
            user.STATE = false;
            _context.Update(user);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0; ;
        }

        public async Task<bool> RemoveUser(int userId)
        {
            var user = await _context.Users.AsNoTracking().SingleOrDefaultAsync(x => x.PK_USER.Equals(userId));

            user!.AUDIT_DELETE_USER = 1;
            user.AUDIT_DELETE_DATE = DateTime.Now;
            user.STATE = false;

            _context.Update(user);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }
    }
}
