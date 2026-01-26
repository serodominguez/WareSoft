using Application.Commons.Bases.Request;
using Application.Dtos.Request.Product;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : BaseApiController
    {
        private readonly IProductService _productService;
        private readonly IGenerateExcelService _generateExcelService;

        public ProductController(IProductService productService, IGenerateExcelService generateExcelService)
        {
            _productService = productService;
            _generateExcelService = generateExcelService;
        }

        [HttpGet]
        [RequirePermission("Productos", "Leer")]
        public async Task<IActionResult> ListProducts([FromQuery] BaseFiltersRequest filters)
        {
            var response = await _productService.ListProducts(filters);

            if ((bool)filters.Download!)
            {
                var columnNames = ExcelColumnNames.GetColumnsProducts();
                var fileBytes = _generateExcelService.GenerateToExcel(response.Data!, columnNames);
                return File(fileBytes, ContentType.ContentTypeExcel);
            }

            return Ok(response);
        }

        [HttpGet("{productId:int}")]
        [RequirePermission("Productos", "Leer")]
        public async Task<IActionResult> ProductById(int productId)
        {
            var response = await _productService.ProductById(productId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Productos", "Crear")]
        public async Task<IActionResult> RegisterProduct([FromBody] ProductRequestDto requestDto)
        {
            var response = await _productService.RegisterProduct(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{productId:int}")]
        [RequirePermission("Productos", "Editar")]
        public async Task<IActionResult> EditProduct(int productId, [FromBody] ProductRequestDto requestDto)
        {
            var response = await _productService.EditProduct(AuthenticatedUserId, productId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{productId:int}")]
        [RequirePermission("Productos", "Editar")]
        public async Task<IActionResult> EnableProduct(int productId)
        {
            var response = await _productService.EnableProduct(AuthenticatedUserId, productId);
            return Ok(response);
        }

        [HttpPut("Disable/{productId:int}")]
        [RequirePermission("Productos", "Editar")]
        public async Task<IActionResult> DisableProduct(int productId)
        {
            var response = await _productService.DisableProduct(AuthenticatedUserId, productId);
            return Ok(response);
        }

        [HttpPut("Remove/{productId:int}")]
        [RequirePermission("Productos", "Eliminar")]
        public async Task<IActionResult> RemoveProduct(int productId)
        {
            var response = await _productService.RemoveProduct(AuthenticatedUserId, productId);
            return Ok(response);
        }
    }
}
