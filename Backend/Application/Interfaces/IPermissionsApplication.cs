using Application.Commons.Bases.Response;
using Application.Dtos.Response.Permissions;

namespace Application.Interfaces
{
    public interface IPermissionsApplication
    {
        Task<bool> UserPermissionAsync(int userId, string moduleName, string actionName);
        Task<BaseResponse<IEnumerable<PermissionsResponseDto>>> GetUserPermissionsAsync(int userId);
    }
}
