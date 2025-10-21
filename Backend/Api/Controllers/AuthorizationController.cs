using Application.Dtos.Request.Users;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthorizationApplication _authorizationApplication;

        public AuthorizationController(IAuthorizationApplication authorizationApplication)
        {
            _authorizationApplication = authorizationApplication;
        }

        [HttpPost("Generate/Token")]
        public async Task<IActionResult> GenerateToken([FromBody] TokenRequestDto requestDto)
        {
            var response = await _authorizationApplication.GenerateToken(requestDto);
            return Ok(response);
        }
    }
}
