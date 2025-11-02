using Application.Commons.Bases.Request;
using Application.Dtos.Request.Roles;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class RolesController : BaseApiController
    {
        private readonly IRolesService _rolesService;
        private readonly IGenerateExcelService _generateExcelService;

        public RolesController(IRolesService rolesService, IGenerateExcelService generateExcelService)
        {
            _rolesService = rolesService;
            _generateExcelService = generateExcelService;
        }

        [HttpGet]
        [RequirePermission("Roles", "Leer")]
        public async Task<IActionResult> ListRoles([FromQuery] BaseFiltersRequest filters)
        {
            var response = await _rolesService.ListRoles(filters);

            if ((bool)filters.Download!)
            {
                var columnNames = ExcelColumnNames.GetColumnsRoles();
                var fileBytes = _generateExcelService.GenerateToExcel(response.Data!, columnNames);
                return File(fileBytes, ContentType.ContentTypeExcel);
            }

            return Ok(response);
        }

        [HttpGet("Select")]
        [RequirePermission("Roles", "Leer")]
        public async Task<IActionResult> ListSelectRoles()
        {
            var response = await _rolesService.ListSelectRoles();
            return Ok(response);
        }

        [HttpGet("{categoryId:int}")]
        [RequirePermission("Roles", "Leer")]
        public async Task<IActionResult> RoleById(int roleId)
        {
            var response = await _rolesService.RoleById(roleId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Roles", "Crear")]
        public async Task<IActionResult> RegisterRole([FromBody] RolesRequestDto requestDto)
        {
            var response = await _rolesService.RegisterRole(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{roleId:int}")]
        [RequirePermission("Roles", "Editar")]
        public async Task<IActionResult> EditRole(int roleId, [FromBody] RolesRequestDto requestDto)
        {
            var response = await _rolesService.EditRole(AuthenticatedUserId, roleId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{roleId:int}")]
        [RequirePermission("Roles", "Editar")]
        public async Task<IActionResult> EnableRole(int roleId)
        {
            var response = await _rolesService.EnableRole(AuthenticatedUserId, roleId);
            return Ok(response);
        }

        [HttpPut("Disable/{roleId:int}")]
        [RequirePermission("Roles", "Editar")]
        public async Task<IActionResult> DisableRole(int roleId)
        {
            var response = await _rolesService.DisableRole(AuthenticatedUserId, roleId);
            return Ok(response);
        }

        [HttpPut("Remove/{roleId:int}")]
        [RequirePermission("Roles", "Eliminar")]
        public async Task<IActionResult> RemoveRole(int roleId)
        {
            var response = await _rolesService.RemoveRole(AuthenticatedUserId, roleId);
            return Ok(response);
        }
    }
}
