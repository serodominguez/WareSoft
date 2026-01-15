using Application.Commons.Bases.Request;
using Application.Dtos.Request.Inventory;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class InventoryController : BaseApiController
    {
        private readonly IInventoryService _inventoryService;
        private readonly IGenerateExcelService _generateExcelService;

        public InventoryController(IInventoryService inventoryService, IGenerateExcelService generateExcelService)
        {
            _inventoryService = inventoryService;
            _generateExcelService = generateExcelService;
        }

        [HttpGet]
        [RequirePermission("Inventario", "Leer")]
        public async Task<IActionResult> ListInventory([FromQuery] BaseFiltersRequest filters)
        {
            var response = await _inventoryService.ListInventoryByStore(AuthenticatedUserStoreId, filters);

            if ((bool)filters.Download!)
            {
                var columnNames = ExcelColumnNames.GetColumnsProducts();
                var fileBytes = _generateExcelService.GenerateToExcel(response.Data!, columnNames);
                return File(fileBytes, ContentType.ContentTypeExcel);
            }

            return Ok(response);
        }

        [HttpPut("Edit")]
        [RequirePermission("Inventario", "Editar")]
        public async Task<IActionResult> EditInventory([FromBody] InventoryRequestDto requestDto)
        {
            var response = await _inventoryService.UpdatePriceByProduct(AuthenticatedUserStoreId, AuthenticatedUserId, requestDto);
            return Ok(response);
        }
    }
}
