using Application.Commons.Bases.Response;
using Application.Dtos.Response.Permissions;
using Application.Interfaces;
using Application.Mappers;
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

        public async Task<BaseResponse<IEnumerable<PermissionsResponseDto>>> ListUserPermissions(int userId)
        {
            var response = new BaseResponse<IEnumerable<PermissionsResponseDto>>();
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
                                .Select(PermissionsMap.PermissionsResponseDtoMapping);

                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_USER_WITHOUT_PERMISSIONS;
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
