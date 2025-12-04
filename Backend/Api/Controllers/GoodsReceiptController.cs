using Application.Commons.Bases.Request;
using Application.Dtos.Request.GoodsReceipt;
using Application.Interfaces;
using Application.Security;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class GoodsReceiptController : BaseApiController
    {
        private readonly IGoodsReceiptService _goodsReceiptService;
        private readonly IGenerateExcelService _generateExcelService;

        public GoodsReceiptController(IGoodsReceiptService goodsReceiptService, IGenerateExcelService generateExcelService)
        {
            _goodsReceiptService = goodsReceiptService;
            _generateExcelService = generateExcelService;
        }

        [HttpGet]
        [RequirePermission("Ingreso de Productos", "Leer")]
        public async Task<IActionResult> ListGoodsReceipt([FromQuery] BaseFiltersRequest filters)
        {
            var response = await _goodsReceiptService.ListGoodsReceipt(filters);

            if ((bool)filters.Download!)
            {
                var columnNames = ExcelColumnNames.GetColumnsGoodsReceipt();
                var fileBytes = _generateExcelService.GenerateToExcel(response.Data!, columnNames);
                return File(fileBytes, ContentType.ContentTypeExcel);
            }

            return Ok(response);
        }

        [HttpGet("{receiptId:int}")]
        [RequirePermission("Ingreso de Productos", "Leer")]
        public async Task<IActionResult> GoodsReceiptById(int receiptId)
        {
            var response = await _goodsReceiptService.GoodsReceiptById(receiptId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Ingreso de Productos", "Crear")]
        public async Task<IActionResult> RegisterGoodsReceipt([FromBody] GoodsReceiptRequestDto requestDto)
        {
            var response = await _goodsReceiptService.RegisterGoodsReceipt(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Cancel/{receiptId:int}")]
        [RequirePermission("Ingreso de Productos", "Eliminar")]
        public async Task<IActionResult> CancelGoodsReceipt(int receiptId)
        {
            var response = await _goodsReceiptService.CancelGoodsReceipt(AuthenticatedUserId, receiptId);
            return Ok(response);
        }
    }
}
