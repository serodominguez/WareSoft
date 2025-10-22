using Application.Commons.Bases.Request;
using Application.Dtos.Request.Brands;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class BrandsController : BaseApiController
    {
        private readonly IBrandsApplication _brandsApplication;

        public BrandsController(IBrandsApplication brandsApplication)
        {
            _brandsApplication = brandsApplication;
        }

        [HttpPost]
        public async Task<IActionResult> ListBrands([FromBody] BaseFiltersRequest filters)
        {
            var response = await _brandsApplication.ListBrands(filters);
            return Ok(response);
        }

        //[AllowAnonymous]
        [HttpGet("Select")]
        public async Task<IActionResult> ListSelectBrands()
        {
            var response = await _brandsApplication.ListSelectBrands();
            return Ok(response);
        }

        [HttpGet("{categoryId:int}")]
        public async Task<IActionResult> BrandById(int brandId)
        {
            var response = await _brandsApplication.BrandById(brandId);
            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterBrand([FromBody] BrandsRequestDto requestDto)
        {
            var response = await _brandsApplication.RegisterBrand(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{brandId:int}")]
        public async Task<IActionResult> EditBrand(int brandId, [FromBody] BrandsRequestDto requestDto)
        {
            var response = await _brandsApplication.EditBrand(AuthenticatedUserId, brandId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{brandId:int}")]
        public async Task<IActionResult> EnableBrand(int brandId)
        {
            var response = await _brandsApplication.EnableBrand(AuthenticatedUserId, brandId);
            return Ok(response);
        }

        [HttpPut("Disable/{brandId:int}")]
        public async Task<IActionResult> DisableBrand(int brandId)
        {
            var response = await _brandsApplication.DisableBrand(AuthenticatedUserId, brandId);
            return Ok(response);
        }

        [HttpPut("Remove/{brandId:int}")]
        public async Task<IActionResult> RemoveBrand(int brandId)
        {
            var response = await _brandsApplication.RemoveBrand(AuthenticatedUserId, brandId);
            return Ok(response);
        }
    }
}
