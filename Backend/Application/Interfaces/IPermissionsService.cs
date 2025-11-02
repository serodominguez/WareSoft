using Application.Commons.Bases.Response;
using Application.Dtos.Response.Permissions;

namespace Application.Interfaces
{
    public interface IPermissionsService
    {
        Task<bool> UserPermissions(int userId, string moduleName, string actionName);
        Task<BaseResponse<IEnumerable<PermissionsResponseDto>>> ListUserPermissions(int userId);
    }
}
