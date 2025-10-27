using Application.Commons.Bases.Response;
using Application.Dtos.Response.Permissions;
using Application.Interfaces;
using Infrastructure.Persistences.Interfaces;
using Utilities.Static;

namespace Application.Services
{
    public class PermissionsApplication : IPermissionsApplication
    {
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PermissionsApplication(IPermissionsRepository permissionsRepository, IUsersRepository usersRepository, IUnitOfWork unitOfWork)
        {
            _permissionsRepository = permissionsRepository;
            _usersRepository = usersRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> UserPermissionAsync(int userId, string moduleName, string actionName)
        {
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user == null || !user.STATE) return false;

            return await _permissionsRepository.PermissionAsync(user.PK_ROLE, moduleName, actionName);
        }

        public async Task<BaseResponse<IEnumerable<PermissionsResponseDto>>> GetUserPermissionsAsync(int userId)
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

                var permissions = await _permissionsRepository.GetRolePermissionsAsync(user.PK_ROLE);

                if (permissions != null && permissions.Any())
                {
                    response.Data = permissions
                        .Where(p => p.STATE && p.Modules!.STATE && p.Actions!.STATE)
                        .Select(p => new PermissionsResponseDto
                        {
                            MODULE = p.Modules!.MODULE_NAME!,
                            ACTION = p.Actions!.ACTION_NAME!
                        });

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
