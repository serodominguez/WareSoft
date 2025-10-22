using Application.Commons.Bases.Request;
using Application.Dtos.Request.Categories;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class CategoriesController : BaseApiController
    {
        private readonly ICategoriesApplication _categoriesApplication;

        public CategoriesController(ICategoriesApplication categoriesApplication)
        {
            _categoriesApplication = categoriesApplication;
        }

        [HttpPost]
        public async Task<IActionResult> ListCategories([FromBody] BaseFiltersRequest filters)
        {
            var response = await _categoriesApplication.ListCategories(filters);
            return Ok(response);
        }

        [HttpGet("Select")]
        public async Task<IActionResult> ListSelectCategories()
        {
            var response = await _categoriesApplication.ListSelectCategories();
            return Ok(response);
        }

        [HttpGet("{categoryId:int}")]
        public async Task<IActionResult> CategoryById(int categoryId)
        {
            var response = await _categoriesApplication.CategoryById(categoryId);
            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterCategory([FromBody] CategoriesRequestDto requestDto)
        {

            var response = await _categoriesApplication.RegisterCategory(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{categoryId:int}")]
        public async Task<IActionResult> EditCategory(int categoryId, [FromBody] CategoriesRequestDto requestDto)
        {

            var response = await _categoriesApplication.EditCategory(AuthenticatedUserId, categoryId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{categoryId:int}")]
        public async Task<IActionResult> EnableCategory(int categoryId)
        {

            var response = await _categoriesApplication.EnableCategory(AuthenticatedUserId, categoryId);
            return Ok(response);
        }

        [HttpPut("Disable/{categoryId:int}")]
        public async Task<IActionResult> DisableCategory(int categoryId)
        {

            var response = await _categoriesApplication.DisableCategory(AuthenticatedUserId, categoryId);
            return Ok(response);
        }

        [HttpPut("Remove/{categoryId:int}")]
        public async Task<IActionResult> RemoveCategory(int categoryId)
        {

            var response = await _categoriesApplication.RemoveCategory(AuthenticatedUserId, categoryId);
            return Ok(response);
        }

    }
}
