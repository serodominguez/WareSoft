using Application.Commons.Bases;
using Application.Dtos.Request.Users;
using Application.Dtos.Response.Users;
using Application.Interfaces;
using Application.Mappers;
using FluentValidation;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Interfaces;
using Utilities.Static;

namespace Application.Services
{
    public class UsersApplication : IUsersApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IValidator<UsersRequestDto> _validator;

        public UsersApplication(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, ITokenService tokenService, IValidator<UsersRequestDto> validator)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _validator = validator;
        }

        public async Task<BaseResponse<BaseEntityResponse<UsersResponseDto>>> ListUsers(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<BaseEntityResponse<UsersResponseDto>>();
            var usersEntity = await _unitOfWork.Users.ListUsers(filters);

            if (usersEntity is not null && usersEntity.Items?.Any() == true)
            {
                var mappedItems = usersEntity.Items.Select(UsersMapp.UsersResponseDtoMapping).ToList();
                response.Data = new BaseEntityResponse<UsersResponseDto>
                {
                    TotalRecords = usersEntity.TotalRecords,
                    Items = mappedItems
                };
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<UsersResponseDto>> UserById(int userId)
        {
            var response = new BaseResponse<UsersResponseDto>();
            var user = await _unitOfWork.Users.UserById(userId);

            if (user is not null)
            {
                response.Data = UsersMapp.UsersResponseDtoMapping(user);
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterUser(UsersRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();
            var validationResult = await _validator.ValidateAsync(requestDto);

            if (!validationResult.IsValid)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_VALIDATE;
                response.Errors = validationResult.Errors;
                return response;
            }

            var user = UsersMapp.UsersMapping(requestDto);
            string password = requestDto.PASSWORD!;
            response.Data = await _unitOfWork.Users.RegisterUser(user, password );

            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_FAILED;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> EditUser(int userId, UsersRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();
            var existingUser= await _unitOfWork.Users.UserById(userId);

            if (existingUser is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            var user = UsersMapp.UsersMapping(requestDto);
            user.PK_USER = userId;
            bool update = requestDto.UPDATE_PASSWORD ?? false;
            string password = requestDto.PASSWORD!;
            response.Data = await _unitOfWork.Users.EditUser(user, update, password);

            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_UPDATE;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_FAILED;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> EnableUser(int userId)
        {
            var response = new BaseResponse<bool>();
            var existingUser = await _unitOfWork.Users.UserById(userId);

            if (existingUser is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            response.Data = await _unitOfWork.Users.EnableUser(userId);

            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_ACTIVATE;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_FAILED;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DisableUser(int userId)
        {
            var response = new BaseResponse<bool>();
            var existingUser = await _unitOfWork.Users.UserById(userId);

            if (existingUser is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            response.Data = await _unitOfWork.Users.DisableUser(userId);

            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_ACTIVATE;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_FAILED;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RemoveUser(int userId)
        {
            var response = new BaseResponse<bool>();
            var existingUser = await _unitOfWork.Users.UserById(userId);

            if (existingUser is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            response.Data = await _unitOfWork.Users.RemoveUser(userId);

            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_DELETE;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_FAILED;
            }

            return response;
        }

        public async Task<BaseResponse<string>> GenerateToken(TokenRequestDto requestDto)
        {
            var response = new BaseResponse<string>();
            var user = await _unitOfWork.Users.AccountByUserName(requestDto.UserName!);

            if (user is not null)
            {
                if (!_passwordHasher.VerifyPasswordHash(requestDto.Password!, user.PASSWORD_HASH!, user.PASSWORD_SALT!))
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_INCORRECT_PASSWORD;
                }
                else
                {
                    response.IsSuccess = true;
                    response.Data = _tokenService.GenerateToken(user);
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
