using Application.Commons.Bases.Response;
using Application.Dtos.Request.Permissions;
using Application.Dtos.Response.Permissions;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Infrastructure.Persistences.Interfaces;
using Utilities.Static;

namespace Application.Services
{
    public class PermissionsService : IPermissionsService
    {
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PermissionsService(IPermissionsRepository permissionsRepository, IUsersRepository usersRepository, IUnitOfWork unitOfWork)
        {
            _permissionsRepository = permissionsRepository;
            _usersRepository = usersRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> UserPermissions(int userId, string moduleName, string actionName)
        {
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user == null || !user.STATE) return false;

            return await _permissionsRepository.GetPermissionsAsync(user.PK_ROLE, moduleName, actionName);
        }

        public async Task<BaseResponse<bool>> UpdatePermissions(int authenticatedUserId, List<PermissionsRequestDto> permissionsDto)
        {
            var response = new BaseResponse<bool>();

            try
            {
                if (permissionsDto == null || !permissionsDto.Any())
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                var existingPermissions = await _permissionsRepository
                    .GetByIdsAsync(permissionsDto.Select(p => p.PK_PERMISSION).ToList());

                var permissionsDict = existingPermissions.ToDictionary(p => p.PK_ENTITY);
                var listPermissionsUpdate = new List<Permissions>();
                
                foreach (var permission in permissionsDto)
                {
                    if (!permissionsDict.TryGetValue(permission.PK_PERMISSION, out var existing))
                    {
                        response.IsSuccess = false;
                        response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                        return response;
                    }

                    if (permission.STATE != existing.STATE)
                    {
                        var permissionToUpdate = PermissionsMap.PermissionsMapping(permission);
                        permissionToUpdate.AUDIT_UPDATE_USER = authenticatedUserId;
                        permissionToUpdate.AUDIT_UPDATE_DATE = DateTime.Now;
                        listPermissionsUpdate.Add(permissionToUpdate);
                    }
                }

                if (!listPermissionsUpdate.Any())
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }

                var result = await _permissionsRepository.UpdatePermissionsRangeAsync(listPermissionsUpdate);

                if (result)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_UPDATE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Data = false;
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

        public async Task<BaseResponse<IEnumerable<PermissionsByUserResponseDto>>> ListUserPermissions(int userId)
        {
            var response = new BaseResponse<IEnumerable<PermissionsByUserResponseDto>>();
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null || !user.STATE)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_USER_NOT_FOUND;
                    return response;
                }

                var permissions = await _permissionsRepository.PermissionsByRoleAsync(user.PK_ROLE);

                if (permissions != null && permissions.Any())
                {
                    response.Data = permissions
                                .Where(p => p.STATE && p.Modules!.STATE && p.Actions!.STATE)
                                .Select(PermissionsMap.PermissionsByUserResponseDtoMapping);

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

        public async Task<BaseResponse<IEnumerable<PermissionsByRoleResponseDto>>> PermissionsByRole(int roleId)
        {
            var response = new BaseResponse<IEnumerable<PermissionsByRoleResponseDto>>();
            try
            {
                var role = await _unitOfWork.Users.GetByIdAsync(roleId);
                if (role == null || !role.STATE)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_USER_NOT_FOUND;
                    return response;
                }

                var permissions = await _permissionsRepository.PermissionsByRoleAsync(roleId);

                if (permissions != null && permissions.Any())
                {
                    response.Data = permissions
                                .Where(p => p.Modules!.STATE && p.Actions!.STATE)
                                .Select(PermissionsMap.PermissionsByRoleResponseDtoMapping);

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
    }
}
