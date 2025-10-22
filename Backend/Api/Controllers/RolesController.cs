using Application.Commons.Bases.Request;
using Application.Dtos.Request.Roles;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class RolesController : BaseApiController
    {
        private readonly IRolesApplication _rolesApplication;
        private readonly IGenerateExcelApplication _generateExcelApplication;

        public RolesController(IRolesApplication rolesApplication, IGenerateExcelApplication generateExcelApplication)
        {
            _rolesApplication = rolesApplication;
            _generateExcelApplication = generateExcelApplication;
        }

        [HttpGet]
        public async Task<IActionResult> ListRoles([FromQuery] BaseFiltersRequest filters)
        {
            var response = await _rolesApplication.ListRoles(filters);

            if ((bool)filters.Download!)
            {
                var columnNames = ExcelColumnNames.GetColumnsRoles();
                var fileBytes = _generateExcelApplication.GenerateToExcel(response.Data!, columnNames);
                return File(fileBytes, ContentType.ContentTypeExcel);
            }

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
            var response = await _rolesApplication.RegisterRole(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{roleId:int}")]
        public async Task<IActionResult> EditRole(int roleId, [FromBody] RolesRequestDto requestDto)
        {
            var response = await _rolesApplication.EditRole(AuthenticatedUserId, roleId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{roleId:int}")]
        public async Task<IActionResult> EnableRole(int roleId)
        {
            var response = await _rolesApplication.EnableRole(AuthenticatedUserId, roleId);
            return Ok(response);
        }

        [HttpPut("Disable/{roleId:int}")]
        public async Task<IActionResult> DisableRole(int roleId)
        {
            var response = await _rolesApplication.DisableRole(AuthenticatedUserId, roleId);
            return Ok(response);
        }

        [HttpPut("Remove/{roleId:int}")]
        public async Task<IActionResult> RemoveRole(int roleId)
        {
            var response = await _rolesApplication.RemoveRole(AuthenticatedUserId, roleId);
            return Ok(response);
        }
    }
}
