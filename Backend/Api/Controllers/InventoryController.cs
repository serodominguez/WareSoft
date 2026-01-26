using Application.Commons.Bases.Request;
using Application.Dtos.Request.Inventory;
using Application.Interfaces;
using Application.Security;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class InventoryController : BaseApiController
    {
        private readonly IInventoryService _inventoryService;
        private readonly IGenerateExcelService _generateExcelService;
        private readonly IGeneratePdfService _generatePdfService;

        public InventoryController(IInventoryService inventoryService, IGenerateExcelService generateExcelService, IGeneratePdfService generatePdfService)
        {
            _inventoryService = inventoryService;
            _generateExcelService = generateExcelService;
            _generatePdfService = generatePdfService;
        }

        [HttpGet]
        [RequirePermission("Inventario", "Leer")]
        public async Task<IActionResult> ListInventory([FromQuery] BaseFiltersRequest filters, [FromQuery] string? downloadType = "excel")
        {
            var response = await _inventoryService.ListInventoryByStore(AuthenticatedUserStoreId, filters);

            if ((bool)filters.Download!)
            {
                // Verificar si se solicita PDF
                if (downloadType?.ToLower() == "pdf")
                {
                    var fileBytes = _generatePdfService.InventoryGeneratePdf(response.Data!.ToList(), AuthenticatedUserStoreName);
                    return File(fileBytes, "application/pdf", $"Inventario_{DateTime.Now:yyyyMMdd}.pdf");
                }
                else
                {
                    var columnNames = ExcelColumnNames.GetColumnsInventories();
                    var fileBytes = _generateExcelService.GenerateToExcel(response.Data!, columnNames);
                    return File(fileBytes, ContentType.ContentTypeExcel);
                }
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
