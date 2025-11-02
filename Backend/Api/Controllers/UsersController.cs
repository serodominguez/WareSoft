using Application.Commons.Bases.Request;
using Application.Dtos.Request.Users;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : BaseApiController
    {
        private readonly IUsersService _usersService;
        private readonly IGenerateExcelService _generateExcelService;

        public UsersController(IUsersService usersService, IGenerateExcelService generateExcelService)
        {
            _usersService = usersService;
            _generateExcelService = generateExcelService;
        }


        [HttpGet]
        [RequirePermission("Usuarios", "Listar")]
        public async Task<IActionResult> ListUsers([FromQuery] BaseFiltersRequest filters)
        {
            var response = await _usersService.ListUsers(filters);

            if ((bool)filters.Download!)
            {
                var columnNames = ExcelColumnNames.GetColumnsUsers();
                var fileBytes = _generateExcelService.GenerateToExcel(response.Data!, columnNames);
                return File(fileBytes, ContentType.ContentTypeExcel);
            }

            return Ok(response);
        }

        [HttpGet("{userId:int}")]
        [RequirePermission("Usuarios", "Listar")]
        public async Task<IActionResult> UserById(int userId)
        {
            var response = await _usersService.UserById(userId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Usuarios", "Crear")]
        public async Task<IActionResult> RegisterUser([FromBody] UsersRequestDto requestDto)
        {
            var response = await _usersService.RegisterUser(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{userId:int}")]
        [RequirePermission("Usuarios", "Editar")]
        public async Task<IActionResult> EditUser(int userId, [FromBody] UsersRequestDto requestDto)
        {
            var response = await _usersService.EditUser(AuthenticatedUserId, userId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{userId:int}")]
        [RequirePermission("Usuarios", "Editar")]
        public async Task<IActionResult> EnableUser(int userId)
        {
            var response = await _usersService.EnableUser(AuthenticatedUserId, userId);
            return Ok(response);
        }

        [HttpPut("Disable/{userId:int}")]
        [RequirePermission("Usuarios", "Editar")]
        public async Task<IActionResult> DisableUser(int userId)
        {
            var response = await _usersService.DisableUser(AuthenticatedUserId, userId);
            return Ok(response);
        }

        [HttpPut("Remove/{userId:int}")]
        [RequirePermission("Usuarios", "Eliminar")]
        public async Task<IActionResult> RemoveUser(int userId)
        {
            var response = await _usersService.RemoveUser(AuthenticatedUserId, userId);
            return Ok(response);
        }
    }
}
