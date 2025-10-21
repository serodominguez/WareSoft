using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.Roles;
using Application.Dtos.Response.Roles;
using Application.Interfaces;
using Application.Mappers;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Application.Services
{
    public class RolesApplication : IRolesApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<RolesRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public RolesApplication(IUnitOfWork unitOfWork, IValidator<RolesRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<RolesResponseDto>>> ListRoles(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<RolesResponseDto>>();

            try
            {
                var roles = _unitOfWork.Roles.GetAllQueryable();

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            roles = roles.Where(x => x.ROLE_NAME!.Contains(filters.TextFilter));
                            break;
                    }
                }

                if (filters.StateFilter is not null)
                {
                    var stateValue = Convert.ToBoolean(filters.StateFilter);
                    roles = roles.Where(x => x.STATE == stateValue);
                }

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);

                    roles = roles.Where(x => x.AUDIT_CREATE_DATE >= startDate && x.AUDIT_CREATE_DATE < endDate);
                }

                filters.Sort ??= "PK_ROLE";
                var items = await _orderingQuery.Ordering(filters, roles, !(bool)filters.Download!).ToListAsync();
                response.IsSuccess = true;
                response.TotalRecords = await roles.CountAsync();
                response.Data = items.Select(RolesMapp.RolesResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<RolesSelectResponseDto>>> ListSelectRoles()
        {
            var response = new BaseResponse<IEnumerable<RolesSelectResponseDto>>();

            try
            {
                var roles = await _unitOfWork.Roles.GetSelectAsync();

                if (roles is not null && roles.Any())
                {
                    response.Data = roles.Select(RolesMapp.RolesSelectResponseDtoMapping);
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
        public async Task<BaseResponse<RolesResponseDto>> RoleById(int roleId)
        {
            var response = new BaseResponse<RolesResponseDto>();

            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId);

                if (role is not null)
                {
                    response.Data = RolesMapp.RolesResponseDtoMapping(role);
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

        public async Task<BaseResponse<bool>> RegisterRole(RolesRequestDto requestDto)
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
                var role = RolesMapp.RolesMapping(requestDto);
                response.Data = await _unitOfWork.Roles.RegisterAsync(role);

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

        public async Task<BaseResponse<bool>> EditRole(int roleId, RolesRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var existingRole = await _unitOfWork.Roles.GetByIdAsync(roleId);

                if (existingRole is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
                var role = RolesMapp.RolesMapping(requestDto);
                role.PK_ROLE = roleId;
                response.Data = await _unitOfWork.Roles.EditAsync(role);

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

        public async Task<BaseResponse<bool>> EnableRole(int roleId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var existingRole = await _unitOfWork.Roles.GetByIdAsync(roleId);

                if (existingRole is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
                response.Data = await _unitOfWork.Roles.EnableAsync(roleId);

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

        public async Task<BaseResponse<bool>> DisableRole(int roleId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var existingRole = await _unitOfWork.Roles.GetByIdAsync(roleId);

                if (existingRole is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
                response.Data = await _unitOfWork.Roles.DisableAsync(roleId);

                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_INACTIVATE;
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

        public async Task<BaseResponse<bool>> RemoveRole(int roleId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var existingRole = await _unitOfWork.Roles.GetByIdAsync(roleId);

                if (existingRole is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
                response.Data = await _unitOfWork.Roles.RemoveAsync(roleId);

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
    }
}
