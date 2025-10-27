using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IPermissionsRepository
    {
        Task<bool> PermissionAsync(int roleId, string moduleName, string actionName);
        Task<IEnumerable<Permissions>> GetRolePermissionsAsync(int roleId);
    }
}
