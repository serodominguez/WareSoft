using Application.Commons.Bases.Response;
using Application.Dtos.Request.Users;

namespace Application.Interfaces
{
    public interface IAuthorizationApplication
    {
        Task<BaseResponse<string>> GenerateToken(TokenRequestDto requestDto);
    }
}