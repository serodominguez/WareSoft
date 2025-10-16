using Application.Commons.Bases;
using Application.Dtos.Request.Users;
using Application.Dtos.Response.Users;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface IUsersApplication
    {
        Task<BaseResponse<BaseEntityResponse<UsersResponseDto>>> ListUsers(BaseFiltersRequest filters);
        Task<BaseResponse<UsersResponseDto>> UserById(int userId);
        Task<BaseResponse<bool>> RegisterUser(UsersRequestDto requestDto);
        Task<BaseResponse<bool>> EditUser(int userId, UsersRequestDto requestDto);
        Task<BaseResponse<bool>> EnableUser(int userId);
        Task<BaseResponse<bool>> DisableUser(int userId);
        Task<BaseResponse<bool>> RemoveUser(int userId);
    }
}
