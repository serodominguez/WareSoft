using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class PermissionsController : BaseApiController
    {
        private readonly IPermissionsApplication _permissionsApplication;

        public PermissionsController(IPermissionsApplication permissionsApplication)
        {
            _permissionsApplication = permissionsApplication;
        }

        [HttpGet("User/{userId:int}")]
        public async Task<IActionResult> GetUserPermissions(int userId)
        {
            var response = await _permissionsApplication.GetUserPermissionsAsync(userId);
            return Ok(response);
        }
    }
}
