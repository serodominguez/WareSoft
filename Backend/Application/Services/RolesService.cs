using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.Roles;
using Application.Dtos.Response.Roles;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Application.Services
{
    public class RolesService : IRolesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActionsRepository _actionsRepository;
        private readonly IModulesRepository _modulesRepository;
        private readonly IValidator<RolesRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public RolesService(IUnitOfWork unitOfWork, IActionsRepository actionsRepository, IModulesRepository modulesRepository, IValidator<RolesRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _actionsRepository = actionsRepository;
            _modulesRepository = modulesRepository;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<RolesResponseDto>>> ListRoles(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<RolesResponseDto>>();

            try
            {
                var roles = _unitOfWork.Roles.GetAllQueryable()
                                       .Where(r => r.AUDIT_DELETE_USER == null && r.AUDIT_DELETE_DATE == null);

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
                response.TotalRecords = await roles.CountAsync();

                filters.Sort ??= "PK_ENTITY";
                var items = await _orderingQuery.Ordering(filters, roles, !(bool)filters.Download!).ToListAsync();
                response.IsSuccess = true;
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
                var roles = (await _unitOfWork.Roles.GetSelectAsync())
                                               .Where(r => r.STATE == true && r.AUDIT_DELETE_USER == null && r.AUDIT_DELETE_DATE == null);

                if (roles is not null && roles.Any())
                {
                    response.Data = roles.Select(RolesMapp.RolesSelectResponseDtoMapping);
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
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
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }


            return response;
        }

        public async Task<BaseResponse<bool>> RegisterRole(int authenticatedUserId, RolesRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();
            using var transaction = _unitOfWork.BeginTransaction();

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

                var entity = RolesMapp.RolesMapping(requestDto);
                entity.AUDIT_CREATE_USER = authenticatedUserId;
                entity.AUDIT_CREATE_DATE = DateTime.Now;
                entity.STATE = true;
                
                var role = await _unitOfWork.Roles.RegisterRoleAsync(entity);
                var actions = (await _actionsRepository.GetActionsAsync())
                                    .Where(x => x.STATE == true).ToList();
                var modules = (await _modulesRepository.GetModulesAsync())
                                    .Where(x => x.STATE == true).ToList();
                var permissions = new List<Permissions>();


                foreach (var module in modules) 
                { 
                    foreach (var action in actions)
                    {
                        permissions.Add(new Permissions
                        {
                            PK_ROLE = role.PK_ENTITY,
                            PK_MODULE = module.PK_ENTITY,
                            PK_ACTION = action.PK_ENTITY,
                            AUDIT_CREATE_USER = authenticatedUserId,
                            AUDIT_CREATE_DATE = DateTime.Now,
                            STATE = false
                        });
                    }
                }

                await _unitOfWork.Permissions.RegisterPermissionsAsync(permissions);
                transaction.Commit();
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> EditRole(int authenticatedUserId, int roleId, RolesRequestDto requestDto)
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

                var isValid = await _unitOfWork.Roles.GetByIdAsync(roleId);
                if (isValid is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                var role = RolesMapp.RolesMapping(requestDto);
                role.PK_ENTITY = roleId;
                role.AUDIT_UPDATE_USER = authenticatedUserId;
                role.AUDIT_UPDATE_DATE = DateTime.Now;

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

        public async Task<BaseResponse<bool>> EnableRole(int authenticatedUserId, int roleId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId);

                if (role is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                role.AUDIT_UPDATE_USER = authenticatedUserId;
                role.AUDIT_UPDATE_DATE = DateTime.Now;
                role.STATE = true;

                response.Data = await _unitOfWork.Roles.UpdateAsync(role);

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

        public async Task<BaseResponse<bool>> DisableRole(int authenticatedUserId, int roleId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId);

                if (role is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                role.AUDIT_UPDATE_USER = authenticatedUserId;
                role.AUDIT_UPDATE_DATE = DateTime.Now;
                role.STATE = false;

                response.Data = await _unitOfWork.Roles.UpdateAsync(role);

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

        public async Task<BaseResponse<bool>> RemoveRole(int authenticatedUserId, int roleId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId);

                if (role is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                role.AUDIT_DELETE_USER = authenticatedUserId;
                role.AUDIT_DELETE_DATE = DateTime.Now;
                role.STATE = false;

                response.Data = await _unitOfWork.Roles.RemoveAsync(role);

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
