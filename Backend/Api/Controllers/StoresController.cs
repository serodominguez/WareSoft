using Application.Commons.Bases.Request;
using Application.Dtos.Request.Stores;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class StoresController : ControllerBase
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
            var response = await _storesApplication.RegisterStore(requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{storeId:int}")]
        public async Task<IActionResult> EditStore(int storeId, [FromBody] StoresRequestDto requestDto)
        {
            var response = await _storesApplication.EditStore(storeId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{storeId:int}")]
        public async Task<IActionResult> EnableStore(int storeId)
        {
            var response = await _storesApplication.EnableStore(storeId);
            return Ok(response);
        }

        [HttpPut("Disable/{storeId:int}")]
        public async Task<IActionResult> DisableStore(int storeId)
        {
            var response = await _storesApplication.DisableStore(storeId);
            return Ok(response);
        }

        [HttpPut("Remove/{storeId:int}")]
        public async Task<IActionResult> RemoveStore(int storeId)
        {
            var response = await _storesApplication.RemoveStore(storeId);
            return Ok(response);
        }
    }
}
