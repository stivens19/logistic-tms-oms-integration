using Microsoft.AspNetCore.Mvc;
using Scharff.Test.Application.Interfaces;

namespace Scharff.Test.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        public AuthController(ITokenService tokenService, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _configuration = configuration;
        }

        [HttpPost("token")]
        public IActionResult GenerateToken([FromBody] AuthRequest request)
        {
            var validClientId = _configuration["Credentials:AUTH_CLIENT_ID"];
            var validClientSecret = _configuration["Credentials:AUTH_CLIENT_SECRET"];
            if (string.IsNullOrEmpty(validClientId) || string.IsNullOrEmpty(validClientSecret))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Las credenciales del servicio no están configuradas en el entorno."
                });
            }

            if (request.ClientId == validClientId && request.ClientSecret == validClientSecret)
            {
                var token = _tokenService.GenerateToken(request.ClientId, "IntegrationService");
                return Ok(new { access_token = token, token_type = "Bearer" });
            }

            return Unauthorized(new { message = "Credenciales de integración inválidas." });
        }
    }

    public record AuthRequest(string ClientId, string ClientSecret);
}
