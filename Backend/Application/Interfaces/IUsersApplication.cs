using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.Users;
using Application.Dtos.Response.Users;

namespace Application.Interfaces
{
    public interface IUsersApplication
    {
        Task<BaseResponse<IEnumerable<UsersResponseDto>>> ListUsers(BaseFiltersRequest filters);
        Task<BaseResponse<UsersResponseDto>> UserById(int userId);
        Task<BaseResponse<bool>> RegisterUser(int authenticatedUserId, UsersRequestDto requestDto);
        Task<BaseResponse<bool>> EditUser(int authenticatedUserId, int userId, UsersRequestDto requestDto);
        Task<BaseResponse<bool>> EnableUser(int authenticatedUserId, int userId);
        Task<BaseResponse<bool>> DisableUser(int authenticatedUserId, int userId);
        Task<BaseResponse<bool>> RemoveUser(int authenticatedUserId, int userId);
    }
}
