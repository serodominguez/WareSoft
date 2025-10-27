using Application.Interfaces;
using Infrastructure.Persistences.Interfaces;

namespace Application.Services
{
    public class PermissionsApplication : IPermissionsApplication
    {
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly IUsersRepository _usersRepository;

        public PermissionsApplication(IPermissionsRepository permissionsRepository, IUsersRepository usersRepository)
        {
            _permissionsRepository = permissionsRepository;
            _usersRepository = usersRepository;
        }

        public async Task<bool> UserPermissionAsync(int userId, string moduleName, string actionName)
        {
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user == null || !user.STATE) return false;

            return await _permissionsRepository.PermissionAsync(user.PK_ROLE, moduleName, actionName);
        }
    }
}
