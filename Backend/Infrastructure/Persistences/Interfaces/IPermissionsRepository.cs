using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IPermissionsRepository : IGenericRepository<Permissions>
    {
        Task<bool> GetPermissionsAsync(int roleId, string moduleName, string actionName);
        Task<IEnumerable<Permissions>> PermissionsByRoleAsync(int roleId);
        Task<bool> RegisterPermissionsAsync(List<Permissions> permissions);
    }
}
