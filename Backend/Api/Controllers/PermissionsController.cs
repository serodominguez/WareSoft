using Application.Interfaces;
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
        public async Task<IActionResult> UserPermissions(int userId)
        {
            var response = await _permissionsService.ListUserPermissions(userId);
            return Ok(response);
        }
    }
}
