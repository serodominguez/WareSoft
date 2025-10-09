using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IRolesRepository
    {
        Task<BaseEntityResponse<Roles>> ListRoles(BaseFiltersRequest filters);
        Task<IEnumerable<Roles>> ListSelectRoles();
        Task<Roles> RoleById(int roleId);
        Task<bool> RegisterRole(Roles role);
        Task<bool> EditRole(Roles role);
        Task<bool> EnableRole(int roleId);
        Task<bool> DisableRole(int roleId);
        Task<bool> RemoveRole(int roleId);
    }
}
