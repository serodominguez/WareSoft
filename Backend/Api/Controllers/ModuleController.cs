using Application.Commons.Bases.Request;
using Application.Dtos.Request.Module;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class ModuleController : BaseApiController
    {
        private readonly IModuleService _moduleService;
        private readonly IGenerateExcelService _generateExcelService;

        public ModuleController(IModuleService moduleService, IGenerateExcelService generateExcelService)
        {
            _moduleService = moduleService;
            _generateExcelService = generateExcelService;
        }

        [HttpGet]
        [RequirePermission("Módulos", "Leer")]
        public async Task<IActionResult> ListModules([FromQuery] BaseFiltersRequest filters)
        {
            var response = await _moduleService.ListModules(filters);

            if ((bool)filters.Download!)
            {
                var columnNames = ExcelColumnNames.GetColumnsModules();
                var fileBytes = _generateExcelService.GenerateToExcel(response.Data!, columnNames);
                return File(fileBytes, ContentType.ContentTypeExcel);
            }

            return Ok(response);
        }

        [HttpGet("{moduleId:int}")]
        [RequirePermission("Módulos", "Leer")]
        public async Task<IActionResult> ModuleById(int moduleId)
        {
            var response = await _moduleService.ModuleById(moduleId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Módulos", "Crear")]
        public async Task<IActionResult> RegisterModule([FromBody] ModuleRequestDto requestDto)
        {
            var response = await _moduleService.RegisterModule(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{moduleId:int}")]
        [RequirePermission("Módulos", "Editar")]
        public async Task<IActionResult> EditModule(int moduleId, [FromBody] ModuleRequestDto requestDto)
        {
            var response = await _moduleService.EditModule(AuthenticatedUserId, moduleId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{moduleId:int}")]
        [RequirePermission("Módulos", "Editar")]
        public async Task<IActionResult> EnableModule(int moduleId)
        {
            var response = await _moduleService.EnableModule(AuthenticatedUserId, moduleId);
            return Ok(response);
        }

        [HttpPut("Disable/{moduleId:int}")]
        [RequirePermission("Módulos", "Editar")]
        public async Task<IActionResult> DisableModule(int moduleId)
        {
            var response = await _moduleService.DisableModule(AuthenticatedUserId, moduleId);
            return Ok(response);
        }

        [HttpPut("Remove/{moduleId:int}")]
        [RequirePermission("Módulos", "Eliminar")]
        public async Task<IActionResult> RemoveModule(int moduleId)
        {
            var response = await _moduleService.RemoveModule(AuthenticatedUserId, moduleId);
            return Ok(response);
        }
    }
}
