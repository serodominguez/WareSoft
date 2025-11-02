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
                .Where(p => p.PK_ROLE == roleId && p.STATE)
                .ToListAsync();
        }

        public async Task<bool> RegisterPermissionsAsync(List<Permissions> permissions)
        {
            await _context.Permissions.AddRangeAsync(permissions);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }
    }
}
