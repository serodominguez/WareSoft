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
        Task<BaseResponse<bool>> RegisterUser(UsersRequestDto requestDto);
        Task<BaseResponse<bool>> EditUser(int userId, UsersRequestDto requestDto);
        Task<BaseResponse<bool>> EnableUser(int userId);
        Task<BaseResponse<bool>> DisableUser(int userId);
        Task<BaseResponse<bool>> RemoveUser(int userId);
    }
}
