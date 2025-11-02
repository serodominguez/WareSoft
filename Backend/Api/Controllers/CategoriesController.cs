using Application.Commons.Bases.Request;
using Application.Dtos.Request.Categories;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class CategoriesController : BaseApiController
    {
        private readonly ICategoriesService _categoriesService;
        private readonly IGenerateExcelService _generateExcelService;

        public CategoriesController(ICategoriesService categoriesService, IGenerateExcelService generateExcelService)
        {
            _categoriesService = categoriesService;
            _generateExcelService = generateExcelService;
        }

        [HttpGet]
        [RequirePermission("Categorías", "Leer")]
        public async Task<IActionResult> ListCategories([FromQuery] BaseFiltersRequest filters)
        {
            var response = await _categoriesService.ListCategories(filters);

            if ((bool)filters.Download!)
            {
                var columnNames = ExcelColumnNames.GetColumnsCategories();
                var fileBytes = _generateExcelService.GenerateToExcel(response.Data!, columnNames);
                return File(fileBytes, ContentType.ContentTypeExcel);
            }

            return Ok(response);
        }

        [HttpGet("Select")]
        [RequirePermission("Categorías", "Leer")]
        public async Task<IActionResult> ListSelectCategories()
        {
            var response = await _categoriesService.ListSelectCategories();
            return Ok(response);
        }

        [HttpGet("{categoryId:int}")]
        [RequirePermission("Categorías", "Leer")]
        public async Task<IActionResult> CategoryById(int categoryId)
        {
            var response = await _categoriesService.CategoryById(categoryId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Categorías", "Crear")]
        public async Task<IActionResult> RegisterCategory([FromBody] CategoriesRequestDto requestDto)
        {

            var response = await _categoriesService.RegisterCategory(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{categoryId:int}")]
        [RequirePermission("Categorías", "Editar")]
        public async Task<IActionResult> EditCategory(int categoryId, [FromBody] CategoriesRequestDto requestDto)
        {

            var response = await _categoriesService.EditCategory(AuthenticatedUserId, categoryId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{categoryId:int}")]
        [RequirePermission("Categorías", "Editar")]
        public async Task<IActionResult> EnableCategory(int categoryId)
        {

            var response = await _categoriesService.EnableCategory(AuthenticatedUserId, categoryId);
            return Ok(response);
        }

        [HttpPut("Disable/{categoryId:int}")]
        [RequirePermission("Categorías", "Editar")]
        public async Task<IActionResult> DisableCategory(int categoryId)
        {

            var response = await _categoriesService.DisableCategory(AuthenticatedUserId, categoryId);
            return Ok(response);
        }

        [HttpPut("Remove/{categoryId:int}")]
        [RequirePermission("Categorías", "Eliminar")]
        public async Task<IActionResult> RemoveCategory(int categoryId)
        {

            var response = await _categoriesService.RemoveCategory(AuthenticatedUserId, categoryId);
            return Ok(response);
        }

    }
}
