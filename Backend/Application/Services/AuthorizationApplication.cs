using Application.Commons.Bases.Response;
using Application.Dtos.Request.Users;
using Application.Interfaces;
using Application.Security;
using Infrastructure.Persistences.Interfaces;
using Utilities.Static;

namespace Application.Services
{
    public class AuthorizationApplication : IAuthorizationApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISecurity _security;

        public AuthorizationApplication(IUnitOfWork unitOfWork, ISecurity security)
        {
            _unitOfWork = unitOfWork;
            _security = security;
        }

        public async Task<BaseResponse<string>> GenerateToken(TokenRequestDto requestDto)
        {
            var response = new BaseResponse<string>();
            var user = await _unitOfWork.Users.AccountByUserName(requestDto.UserName!);

            if (user is not null)
            {
                if (!_security.VerifyPasswordHash(requestDto.Password!, user.PASSWORD_HASH!, user.PASSWORD_SALT!))
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_INCORRECT_PASSWORD;
                }
                else
                {
                    response.IsSuccess = true;
                    response.Data = _security.GenerateToken(user);
                    response.Message = ReplyMessage.MESSAGE_TOKEN;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_INCORRECT_USER;
            }

            return response;
        }
    }
}
