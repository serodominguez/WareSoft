using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.Modules;
using Application.Dtos.Response.Modules;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Application.Services
{
    public class ModulesService : IModulesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActionsRepository _actionsRepository;
        private readonly IRolesRepository _rolesRepository;
        private readonly IValidator<ModulesRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public ModulesService(IUnitOfWork unitOfWork, IActionsRepository actionsRepository, IRolesRepository rolesRepository, IValidator<ModulesRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _actionsRepository = actionsRepository;
            _rolesRepository = rolesRepository;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }
        public async Task<BaseResponse<IEnumerable<ModulesResponseDto>>> ListModules(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<ModulesResponseDto>>();

            try
            {
                var modules = _unitOfWork.Modules.GetAllQueryable()
                                       .Where(m => m.AUDIT_DELETE_USER == null && m.AUDIT_DELETE_DATE == null);

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            modules = modules.Where(x => x.MODULE_NAME!.Contains(filters.TextFilter));
                            break;
                    }
                }

                if (filters.StateFilter is not null)
                {
                    var stateValue = Convert.ToBoolean(filters.StateFilter);
                    modules = modules.Where(x => x.STATE == stateValue);
                }

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);

                    modules = modules.Where(x => x.AUDIT_CREATE_DATE >= startDate && x.AUDIT_CREATE_DATE < endDate);
                }
                response.TotalRecords = await modules.CountAsync();

                filters.Sort ??= "PK_ENTITY";
                var items = await _orderingQuery.Ordering(filters, modules, !(bool)filters.Download!).ToListAsync();
                response.IsSuccess = true;
                response.Data = items.Select(ModulesMapp.ModulesResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<ModulesResponseDto>> ModuleById(int moduleId)
        {

            var response = new BaseResponse<ModulesResponseDto>();

            try
            {
                var module = await _unitOfWork.Modules.GetByIdAsync(moduleId);

                if (module is not null)
                {
                    response.Data = ModulesMapp.ModulesResponseDtoMapping(module);
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

        public async Task<BaseResponse<bool>> RegisterModule(int authenticatedUserId, ModulesRequestDto requestDto)
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

                var entity = ModulesMapp.ModulesMapping(requestDto);
                entity.AUDIT_CREATE_USER = authenticatedUserId;
                entity.AUDIT_CREATE_DATE = DateTime.Now;
                entity.STATE = true;

                var module = await _unitOfWork.Modules.RegisterModuleAsync(entity);
                var actions = (await _actionsRepository.GetActionsAsync())
                                    .Where(x => x.STATE == true).ToList();
                var roles = (await _rolesRepository.GetRolesAsync())
                                    .Where(x => x.STATE == true).ToList();
                var permissions = new List<Permissions>();


                foreach (var role in roles)
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

        public async Task<BaseResponse<bool>> EditModule(int authenticatedUserId, int moduleId, ModulesRequestDto requestDto)
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

                var isValid = await _unitOfWork.Modules.GetByIdAsync(moduleId);
                if (isValid is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                var module = ModulesMapp.ModulesMapping(requestDto);
                module.PK_ENTITY = moduleId;
                module.AUDIT_UPDATE_USER = authenticatedUserId;
                module.AUDIT_UPDATE_DATE = DateTime.Now;

                response.Data = await _unitOfWork.Modules.EditAsync(module);

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

        public async Task<BaseResponse<bool>> EnableModule(int authenticatedUserId, int moduleId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var module = await _unitOfWork.Modules.GetByIdAsync(moduleId);

                if (module is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                module.AUDIT_UPDATE_USER = authenticatedUserId;
                module.AUDIT_UPDATE_DATE = DateTime.Now;
                module.STATE = true;

                response.Data = await _unitOfWork.Modules.UpdateAsync(module);

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

        public async Task<BaseResponse<bool>> DisableModule(int authenticatedUserId, int moduleId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var module = await _unitOfWork.Modules.GetByIdAsync(moduleId);

                if (module is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                module.AUDIT_UPDATE_USER = authenticatedUserId;
                module.AUDIT_UPDATE_DATE = DateTime.Now;
                module.STATE = false;

                response.Data = await _unitOfWork.Modules.UpdateAsync(module);

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

        public async Task<BaseResponse<bool>> RemoveModule(int authenticatedUserId, int moduleId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var module = await _unitOfWork.Modules.GetByIdAsync(moduleId);

                if (module is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                module.AUDIT_DELETE_USER = authenticatedUserId;
                module.AUDIT_DELETE_DATE = DateTime.Now;
                module.STATE = false;

                response.Data = await _unitOfWork.Modules.RemoveAsync(module);

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
