using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class PermissionsRepository : GenericRepository<Permissions>, IPermissionsRepository
    {
        private readonly DbContextSystem _context;
        private readonly DbSet<Permissions> _entity;
        //private readonly IMemoryCache _cache;

        public PermissionsRepository(DbContextSystem context) : base(context)
        {
            _context = context;
            _entity = _context.Set<Permissions>();
        }

        public async Task<bool> GetPermissionsAsync(int roleId, string moduleName, string actionName)
        {
            //Consulta con cache

            //var cacheKey = $"permission_{roleId}_{moduleName}_{actionName}";

            //if (_cache.TryGetValue(cacheKey, out bool cachedResult))
            //    return cachedResult;

            //var hasPermission = await _context.Permissions
            //    .Include(p => p.Modules)
            //    .Include(p => p.Actions)
            //    .AnyAsync(p =>
            //        p.PK_ROLE == roleId &&
            //        p.Modules!.MODULE_NAME == moduleName &&
            //        p.Actions!.ACTION_NAME == actionName &&
            //        p.STATE &&
            //        p.Modules.STATE &&
            //        p.Actions.STATE
            //    );

            //_cache.Set(cacheKey, hasPermission, TimeSpan.FromMinutes(30));
            //return hasPermission;

            return await _context.Permissions
                    .Include(p => p.Modules)
                    .Include(p => p.Actions)
                    .AnyAsync(p =>
                                p.PK_ROLE == roleId &&
                                p.Modules!.MODULE_NAME == moduleName &&
                                p.Actions!.ACTION_NAME == actionName &&
                                p.STATE &&
                                p.Modules.STATE &&
                                p.Actions.STATE
                );
        }

        public async Task<IEnumerable<Permissions>> PermissionsByRoleAsync(int roleId)
        {
            return await _context.Permissions
                .Include(p => p.Modules)
                .Include(p => p.Actions)
                .Where(p => p.PK_ROLE == roleId)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<Permissions>> GetByIdsAsync(List<int> permissionIds)
        {
            return await _context.Permissions
                 .Where(p => permissionIds.Contains(p.PK_ENTITY))
                 .AsNoTracking()
                 .ToListAsync();
        }

        public async Task<bool> RegisterPermissionsAsync(List<Permissions> permissions)
        {
            await _context.Permissions.AddRangeAsync(permissions);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> UpdatePermissionsRangeAsync(List<Permissions> permissions)
        {
            foreach (var permission in permissions)
            {
                _context.Update(permission);
                _context.Entry(permission).Property(x => x.PK_ROLE).IsModified = false;
                _context.Entry(permission).Property(x => x.PK_MODULE).IsModified = false;
                _context.Entry(permission).Property(x => x.PK_ACTION).IsModified = false;
                _context.Entry(permission).Property(x => x.AUDIT_CREATE_USER).IsModified = false;
                _context.Entry(permission).Property(x => x.AUDIT_CREATE_DATE).IsModified = false;
            }

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }
    }
}
