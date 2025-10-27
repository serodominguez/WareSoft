using Application.Commons.Bases.Request;
using Application.Dtos.Request.Stores;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class StoresController : BaseApiController
    {
        private readonly IStoresApplication _storesApplication;
        private readonly IGenerateExcelApplication _generateExcelApplication;

        public StoresController(IStoresApplication storesApplication, IGenerateExcelApplication generateExcelApplication)
        {
            _storesApplication = storesApplication;
            _generateExcelApplication = generateExcelApplication;
        }

        [HttpGet]
        [RequirePermission("Tiendas", "Leer")]
        public async Task<IActionResult> ListStores([FromQuery] BaseFiltersRequest filters)
        {
            var response = await _storesApplication.ListStores(filters);

            if ((bool)filters.Download!)
            {
                var columnNames = ExcelColumnNames.GetColumnsStores();
                var fileBytes = _generateExcelApplication.GenerateToExcel(response.Data!, columnNames);
                return File(fileBytes, ContentType.ContentTypeExcel);
            }

            return Ok(response);
        }

        [HttpGet("Select")]
        [RequirePermission("Tiendas", "Leer")]
        public async Task<IActionResult> ListSelectStores()
        {
            var response = await _storesApplication.ListSelectStores();
            return Ok(response);
        }

        [HttpGet("{storeId:int}")]
        [RequirePermission("Tiendas", "Leer")]
        public async Task<IActionResult> StoreById(int storeId)
        {
            var response = await _storesApplication.StoreById(storeId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Tiendas", "Crear")]
        public async Task<IActionResult> RegisterStore([FromBody] StoresRequestDto requestDto)
        {
            var response = await _storesApplication.RegisterStore(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{storeId:int}")]
        [RequirePermission("Tiendas", "Editar")]
        public async Task<IActionResult> EditStore(int storeId, [FromBody] StoresRequestDto requestDto)
        {
            var response = await _storesApplication.EditStore(AuthenticatedUserId, storeId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{storeId:int}")]
        [RequirePermission("Tiendas", "Editar")]
        public async Task<IActionResult> EnableStore(int storeId)
        {
            var response = await _storesApplication.EnableStore(AuthenticatedUserId, storeId);
            return Ok(response);
        }

        [HttpPut("Disable/{storeId:int}")]
        [RequirePermission("Tiendas", "Editar")]
        public async Task<IActionResult> DisableStore(int storeId)
        {
            var response = await _storesApplication.DisableStore(AuthenticatedUserId, storeId);
            return Ok(response);
        }

        [HttpPut("Remove/{storeId:int}")]
        [RequirePermission("Tiendas", "Eliminar")]
        public async Task<IActionResult> RemoveStore(int storeId)
        {
            var response = await _storesApplication.RemoveStore(AuthenticatedUserId, storeId);
            return Ok(response);
        }
    }
}
