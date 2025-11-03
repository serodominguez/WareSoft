using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class PermissionsController : BaseApiController
    {
        private readonly IPermissionsService _permissionsService;

        public PermissionsController(IPermissionsService permissionsService)
        {
            _permissionsService = permissionsService;
        }

        [HttpGet("User/{userId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> UserPermissions(int userId)
        {
            var response = await _permissionsService.ListUserPermissions(userId);
            return Ok(response);
        }

        [HttpGet("Role/{roleId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> RolePermissions(int roleId)
        {
            var response = await _permissionsService.PermissionsByRole(roleId);
            return Ok(response);
        }
    }
}
