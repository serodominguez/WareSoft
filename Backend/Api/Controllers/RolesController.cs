using Application.Dtos.Request.Roles;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Commons.Bases.Request;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RolesController : ControllerBase
    {
        private readonly IRolesApplication _rolesApplication;

        public RolesController(IRolesApplication rolesApplication)
        {
            _rolesApplication = rolesApplication;
        }

        [HttpPost]
        public async Task<IActionResult> ListRoles([FromBody] BaseFiltersRequest filters)
        {
            var response = await _rolesApplication.ListRoles(filters);
            return Ok(response);
        }

        [HttpGet("Select")]
        public async Task<IActionResult> ListSelectRoles()
        {
            var response = await _rolesApplication.ListSelectRoles();
            return Ok(response);
        }

        [HttpGet("{categoryId:int}")]
        public async Task<IActionResult> RoleById(int roleId)
        {
            var response = await _rolesApplication.RoleById(roleId);
            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterRole([FromBody] RolesRequestDto requestDto)
        {
            var response = await _rolesApplication.RegisterRole(requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{roleId:int}")]
        public async Task<IActionResult> EditRole(int roleId, [FromBody] RolesRequestDto requestDto)
        {
            var response = await _rolesApplication.EditRole(roleId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{roleId:int}")]
        public async Task<IActionResult> EnableRole(int roleId)
        {
            var response = await _rolesApplication.EnableRole(roleId);
            return Ok(response);
        }

        [HttpPut("Disable/{roleId:int}")]
        public async Task<IActionResult> DisableRole(int roleId)
        {
            var response = await _rolesApplication.DisableRole(roleId);
            return Ok(response);
        }

        [HttpPut("Remove/{roleId:int}")]
        public async Task<IActionResult> RemoveRole(int roleId)
        {
            var response = await _rolesApplication.RemoveRole(roleId);
            return Ok(response);
        }
    }
}
