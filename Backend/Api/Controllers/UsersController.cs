using Application.Commons.Bases.Request;
using Application.Dtos.Request.Users;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : BaseApiController
    {
        private readonly IUsersApplication _usersApplication;
        private readonly IGenerateExcelApplication _generateExcelApplication;

        public UsersController(IUsersApplication usersApplication, IGenerateExcelApplication generateExcelApplication)
        {
            _usersApplication = usersApplication;
            _generateExcelApplication = generateExcelApplication;
        }


        [HttpGet]
        public async Task<IActionResult> ListUsers([FromQuery] BaseFiltersRequest filters)
        {
            var response = await _usersApplication.ListUsers(filters);

            if ((bool)filters.Download!)
            {
                var columnNames = ExcelColumnNames.GetColumnsUsers();
                var fileBytes = _generateExcelApplication.GenerateToExcel(response.Data!, columnNames);
                return File(fileBytes, ContentType.ContentTypeExcel);
            }

            return Ok(response);
        }

        [HttpGet("{userId:int}")]
        public async Task<IActionResult> UserById(int userId)
        {
            var response = await _usersApplication.UserById(userId);
            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] UsersRequestDto requestDto)
        {
            var response = await _usersApplication.RegisterUser(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{userId:int}")]
        public async Task<IActionResult> EditUser(int userId, [FromBody] UsersRequestDto requestDto)
        {
            var response = await _usersApplication.EditUser(AuthenticatedUserId, userId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{userId:int}")]
        public async Task<IActionResult> EnableUser(int userId)
        {
            var response = await _usersApplication.EnableUser(AuthenticatedUserId, userId);
            return Ok(response);
        }

        [HttpPut("Disable/{userId:int}")]
        public async Task<IActionResult> DisableUser(int userId)
        {
            var response = await _usersApplication.DisableUser(AuthenticatedUserId, userId);
            return Ok(response);
        }

        [HttpPut("Remove/{userId:int}")]
        public async Task<IActionResult> RemoveUser(int userId)
        {
            var response = await _usersApplication.RemoveUser(AuthenticatedUserId, userId);
            return Ok(response);
        }
    }
}
