using Application.Commons.Bases.Request;
using Application.Dtos.Request.Modules;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class ModulesController : BaseApiController
    {
        private readonly IModulesService _modulesService;
        private readonly IGenerateExcelService _generateExcelService;

        public ModulesController(IModulesService modulesService, IGenerateExcelService generateExcelService)
        {
            _modulesService = modulesService;
            _generateExcelService = generateExcelService;
        }

        [HttpGet]
        //[AllowAnonymous]
        [RequirePermission("Módulos", "Leer")]
        public async Task<IActionResult> ListModules([FromQuery] BaseFiltersRequest filters)
        {
            var response = await _modulesService.ListModules(filters);

            if ((bool)filters.Download!)
            {
                var columnNames = ExcelColumnNames.GetColumnsModules();
                var fileBytes = _generateExcelService.GenerateToExcel(response.Data!, columnNames);
                return File(fileBytes, ContentType.ContentTypeExcel);
            }

            return Ok(response);
        }

        [HttpGet("{moduleId:int}")]
        //[AllowAnonymous]
        [RequirePermission("Módulos", "Leer")]
        public async Task<IActionResult> ModuleById(int moduleId)
        {
            var response = await _modulesService.ModuleById(moduleId);
            return Ok(response);
        }

        [HttpPost("Register")]
        //[AllowAnonymous]
        [RequirePermission("Módulos", "Crear")]
        public async Task<IActionResult> RegisterModule([FromBody] ModulesRequestDto requestDto)
        {
            var response = await _modulesService.RegisterModule(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{moduleId:int}")]
        //[AllowAnonymous]
        [RequirePermission("Módulos", "Editar")]
        public async Task<IActionResult> EditModule(int moduleId, [FromBody] ModulesRequestDto requestDto)
        {
            var response = await _modulesService.EditModule(AuthenticatedUserId, moduleId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{moduleId:int}")]
        //[AllowAnonymous]
        [RequirePermission("Módulos", "Editar")]
        public async Task<IActionResult> EnableModule(int moduleId)
        {
            var response = await _modulesService.EnableModule(AuthenticatedUserId, moduleId);
            return Ok(response);
        }

        [HttpPut("Disable/{moduleId:int}")]
        //[AllowAnonymous]
        [RequirePermission("Módulos", "Editar")]
        public async Task<IActionResult> DisableModule(int moduleId)
        {
            var response = await _modulesService.DisableModule(AuthenticatedUserId, moduleId);
            return Ok(response);
        }

        [HttpPut("Remove/{moduleId:int}")]
        //[AllowAnonymous]
        [RequirePermission("Módulos", "Eliminar")]
        public async Task<IActionResult> RemoveModule(int moduleId)
        {
            var response = await _modulesService.RemoveModule(AuthenticatedUserId, moduleId);
            return Ok(response);
        }
    }
}
