using Application.Commons.Bases.Request;
using Application.Dtos.Request.Stores;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class StoresController : BaseApiController
    {
        private readonly IStoresApplication _storesApplication;

        public StoresController(IStoresApplication storesApplication)
        {
            _storesApplication = storesApplication;
        }

        [HttpPost]
        public async Task<IActionResult> ListStores([FromBody] BaseFiltersRequest filters)
        {
            var response = await _storesApplication.ListStores(filters);
            return Ok(response);
        }

        [HttpGet("Select")]
        public async Task<IActionResult> ListSelectStores()
        {
            var response = await _storesApplication.ListSelectStores();
            return Ok(response);
        }

        [HttpGet("{storeId:int}")]
        public async Task<IActionResult> StoreById(int storeId)
        {
            var response = await _storesApplication.StoreById(storeId);
            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterStore([FromBody] StoresRequestDto requestDto)
        {
            var response = await _storesApplication.RegisterStore(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{storeId:int}")]
        public async Task<IActionResult> EditStore(int storeId, [FromBody] StoresRequestDto requestDto)
        {
            var response = await _storesApplication.EditStore(AuthenticatedUserId, storeId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{storeId:int}")]
        public async Task<IActionResult> EnableStore(int storeId)
        {
            var response = await _storesApplication.EnableStore(AuthenticatedUserId, storeId);
            return Ok(response);
        }

        [HttpPut("Disable/{storeId:int}")]
        public async Task<IActionResult> DisableStore(int storeId)
        {
            var response = await _storesApplication.DisableStore(AuthenticatedUserId, storeId);
            return Ok(response);
        }

        [HttpPut("Remove/{storeId:int}")]
        public async Task<IActionResult> RemoveStore(int storeId)
        {
            var response = await _storesApplication.RemoveStore(AuthenticatedUserId, storeId);
            return Ok(response);
        }
    }
}
