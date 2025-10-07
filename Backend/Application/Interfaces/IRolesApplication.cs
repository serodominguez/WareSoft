using Application.Commons.Bases;
using Application.Dtos.Request.Roles;
using Application.Dtos.Response.Roles;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface IRolesApplication
    {
        Task<BaseResponse<BaseEntityResponse<RolesResponseDto>>> ListRoles(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<RolesSelectResponseDto>>> ListSelectRoles();
        Task<BaseResponse<RolesResponseDto>> RoleById(int roleId);
        Task<BaseResponse<bool>> RegisterRole(RolesRequestDto requestDto);
        Task<BaseResponse<bool>> EditRole(int roleId, RolesRequestDto requestDto);
        Task<BaseResponse<bool>> EnableRole(int roleId);
        Task<BaseResponse<bool>> DisableRole(int roleId);
        Task<BaseResponse<bool>> RemoveRole(int roleId);
    }
}
