using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Infrastructure.Persistences.Repositories
{
    public class RolesRepository : GenericRepository<Roles>, IRolesRepository
    {
        private readonly DbContextSystem _context;

        public RolesRepository(DbContextSystem context)
        {
            _context = context;
        }
        public async Task<BaseEntityResponse<Roles>> ListRoles(BaseFiltersRequest filters)
        {
            var response = new BaseEntityResponse<Roles>();
            var roles = (from r in _context.Roles
                          where r.AUDIT_DELETE_USER == null && r.AUDIT_DELETE_DATE == null
                          select r).AsNoTracking().AsQueryable();

            if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
            {
                switch (filters.NumberFilter)
                {
                    case 1:
                        roles = roles.Where(x => x.ROLE_NAME!.Contains(filters.TextFilter));
                        break;
                }
            }

            if (filters.StateFilter is not null)
            {
                var stateValue = Convert.ToBoolean(filters.StateFilter);
                roles = roles.Where(x => x.STATE == stateValue);
            }

            if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
            {
                var startDate = Convert.ToDateTime(filters.StartDate).Date;
                var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);

                roles = roles.Where(x => x.AUDIT_CREATE_DATE >= startDate && x.AUDIT_CREATE_DATE <= endDate);
            }

            if (filters.Sort is null) filters.Sort = "PK_ROLE";

            response.TotalRecords = await roles.CountAsync();
            response.Items = await Ordering(filters, roles, !(bool)filters.Download!).ToListAsync();
            return response;
        }

        public async Task<IEnumerable<Roles>> ListSelectRoles()
        {
            var roles = await _context.Roles
                .Where(x => Convert.ToInt32(x.STATE).Equals((int)StateTypes.ACTIVE) && x.AUDIT_DELETE_USER == null && x.AUDIT_DELETE_DATE == null).AsNoTracking().ToListAsync();
            return roles;
        }

        public async Task<Roles> RoleById(int roleId)
        {
            var role = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.PK_ROLE.Equals(roleId));
            return role!;
        }

        public async Task<bool> RegisterRole(Roles role)
        {
            role.AUDIT_CREATE_USER = 1;
            role.AUDIT_CREATE_DATE = DateTime.Now;
            role.STATE = true;

            await _context.AddAsync(role);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> EditRole(Roles role)
        {
            role.AUDIT_UPDATE_USER = 1;
            role.AUDIT_UPDATE_DATE = DateTime.Now;

            _context.Update(role);
            _context.Entry(role).Property(x => x.STATE).IsModified = false;
            _context.Entry(role).Property(x => x.AUDIT_CREATE_USER).IsModified = false;
            _context.Entry(role).Property(x => x.AUDIT_CREATE_DATE).IsModified = false;

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> EnableRole(int roleId)
        {
            var role = await _context.Roles.AsNoTracking().SingleOrDefaultAsync(x => x.PK_ROLE.Equals(roleId));

            role!.AUDIT_UPDATE_USER = 1;
            role.AUDIT_UPDATE_DATE = DateTime.Now;
            role.STATE = true;
            _context.Update(role);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }
        public async Task<bool> DisableRole(int roleId)
        {
            var role = await _context.Roles.AsNoTracking().SingleOrDefaultAsync(x => x.PK_ROLE.Equals(roleId));

            role!.AUDIT_UPDATE_USER = 1;
            role.AUDIT_UPDATE_DATE = DateTime.Now;
            role.STATE = false;
            _context.Update(role);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> RemoveRole(int roleId)
        {
            var role = await _context.Roles.AsNoTracking().SingleOrDefaultAsync(x => x.PK_ROLE.Equals(roleId));

            role!.AUDIT_DELETE_USER = 1;
            role.AUDIT_DELETE_DATE = DateTime.Now;
            role.STATE = false;

            _context.Update(role);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }


    }
}
