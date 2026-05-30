using Atelier.Api._DTOs;
using Atelier.Api._Swagger;
using Atelier.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atelier.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("token")]
        [SwaggerDocumentation(
            summary: "Generate a JWT token",
            description: "Returns a JWT token valid for 8 hours if the credentials are correct")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseType400Dto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseType401Dto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseType500Dto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (result == null)
                return Unauthorized(new ResponseType401Dto ("Missing or invalid token"));

            return Ok(new LoginResponseDto
            {
                Token = result.Value.token,
                Expiration = result.Value.expiration
            });
        }
    }
}
