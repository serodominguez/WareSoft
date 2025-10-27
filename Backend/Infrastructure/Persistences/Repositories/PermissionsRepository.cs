using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class PermissionsRepository : IPermissionsRepository
    {
        private readonly DbContextSystem _context;
        private readonly IMemoryCache _cache;

        public PermissionsRepository(DbContextSystem context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<bool> PermissionAsync(int roleId, string moduleName, string actionName)
        {
            var cacheKey = $"permission_{roleId}_{moduleName}_{actionName}";

            if (_cache.TryGetValue(cacheKey, out bool cachedResult))
                return cachedResult;

            var hasPermission = await _context.Permissions
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

            _cache.Set(cacheKey, hasPermission, TimeSpan.FromMinutes(30));
            return hasPermission;

            //Consulta a la base de datos sin cache

            //return await _context.Permissions
            //    .Include(p => p.Modules)
            //    .Include(p => p.Actions)
            //    .AnyAsync(p =>
            //        p.PK_ROLE == roleId &&
            //        p.Modules.MODULE_NAME == moduleName &&
            //        p.Actions.ACTION_NAME == actionName &&
            //        p.STATE &&
            //        p.Modules.STATE &&
            //        p.Actions.STATE
            //    );
        }

        public async Task<IEnumerable<Permissions>> GetRolePermissionsAsync(int roleId)
        {
            return await _context.Permissions
                .Include(p => p.Modules)
                .Include(p => p.Actions)
                .Where(p => p.PK_ROLE == roleId && p.STATE)
                .ToListAsync();
        }
    }
}
