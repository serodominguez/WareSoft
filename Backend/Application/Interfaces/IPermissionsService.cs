using Application.Commons.Bases.Response;
using Application.Dtos.Request.Permissions;
using Application.Dtos.Response.Permissions;

namespace Application.Interfaces
{
    public interface IPermissionsService
    {
        Task<bool> UserPermissions(int userId, string moduleName, string actionName);
        Task<BaseResponse<bool>> UpdatePermissions(int authenticatedUserId, List<PermissionsRequestDto> permissions);
        Task<BaseResponse<IEnumerable<PermissionsByUserResponseDto>>> ListUserPermissions(int userId);
        Task<BaseResponse<IEnumerable<PermissionsByRoleResponseDto>>> PermissionsByRole(int roleId);
    }
}
