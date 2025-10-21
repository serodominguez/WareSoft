using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.Users;
using Application.Dtos.Response.Users;
using Application.Interfaces;
using Application.Mappers;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Application.Services
{
    public class UsersApplication : IUsersApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISecurity _security;
        private readonly IValidator<UsersRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public UsersApplication(IUnitOfWork unitOfWork, ISecurity security, IValidator<UsersRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _security = security;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<UsersResponseDto>>> ListUsers(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<UsersResponseDto>>();
            try
            {
                var users = _unitOfWork.Users.GetAllQueryable();

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            users = users.Where(x => x.USER_NAME!.Contains(filters.TextFilter));
                            break;
                        case 2:
                            users = users.Where(x => x.NAMES!.Contains(filters.TextFilter));
                            break;
                        case 3:
                            users = users.Where(x => x.LAST_NAMES!.Contains(filters.TextFilter));
                            break;
                        case 4:
                            users = users.Where(x => x.Stores!.STORE_NAME!.Contains(filters.TextFilter));
                            break;
                        case 5:
                            users = users.Where(x => x.Roles!.ROLE_NAME!.Contains(filters.TextFilter));
                            break;
                    }
                }

                if (filters.StateFilter is not null)
                {
                    var stateValue = Convert.ToBoolean(filters.StateFilter);
                    users = users.Where(x => x.STATE == stateValue);
                }

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);
                    users = users.Where(x => x.AUDIT_CREATE_DATE >= startDate && x.AUDIT_CREATE_DATE <= endDate);
                }

                if (filters.Sort is null) filters.Sort = "PK_USERS";
                var items = await _orderingQuery.Ordering(filters, users, !(bool)filters.Download!).ToListAsync();
                response.IsSuccess = true;
                response.TotalRecords = await users.CountAsync();
                response.Data = items.Select(UsersMapp.UsersResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<UsersResponseDto>> UserById(int userId)
        {
            var response = new BaseResponse<UsersResponseDto>();

            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

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
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterUser(UsersRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var validationResult = await _validator.ValidateAsync(requestDto);

                if (!validationResult.IsValid)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_VALIDATE;
                    response.Errors = validationResult.Errors;
                    return response;
                }

                _security.GeneratePasswordHash(requestDto.PASSWORD!, out byte[] passwordHash, out byte[] passwordSalt);
                var user = UsersMapp.UsersMapping(requestDto);
                user.PASSWORD_HASH = passwordHash;
                user.PASSWORD_SALT = passwordSalt;

                response.Data = await _unitOfWork.Users.RegisterAsync(user);
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
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> EditUser(int userId, UsersRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var existingUser = await _unitOfWork.Users.GetByIdAsync(userId);

                if (existingUser is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }

                var user = UsersMapp.UsersMapping(requestDto);
                if (requestDto.UPDATE_PASSWORD == true)
                {
                    _security.GeneratePasswordHash(requestDto.PASSWORD!, out byte[] passwordHash, out byte[] passwordSalt);
                    user.PASSWORD_HASH = passwordHash;
                    user.PASSWORD_SALT = passwordSalt;
                }

                response.Data = await _unitOfWork.Users.EditUser(user, requestDto.UPDATE_PASSWORD);

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
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> EnableUser(int userId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var existingUser = await _unitOfWork.Users.GetByIdAsync(userId);

                if (existingUser is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
                response.Data = await _unitOfWork.Users.EnableAsync(userId);

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
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DisableUser(int userId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var existingUser = await _unitOfWork.Users.GetByIdAsync(userId);

                if (existingUser is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
                response.Data = await _unitOfWork.Users.DisableAsync(userId);

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
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RemoveUser(int userId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var existingUser = await _unitOfWork.Users.GetByIdAsync(userId);

                if (existingUser is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
                response.Data = await _unitOfWork.Users.RemoveAsync(userId);

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
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
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
