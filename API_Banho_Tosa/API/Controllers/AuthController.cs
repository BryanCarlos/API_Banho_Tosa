using API_Banho_Tosa.Application.Auth.DTOs;
using API_Banho_Tosa.Application.Auth.Services;
using API_Banho_Tosa.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_Banho_Tosa.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserCreateRequest request)
        {
            var response = await _authService.RegisterAsync(request);
            return CreatedAtRoute(
                routeName: "GetUserById",
                routeValues: new { id = response.Uuid },
                value: response
            );
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
        {
            var response = await _authService.RefreshTokensAsync(request);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userUuid))
            {
                return BadRequest("Invalid user ID in the token.");
            }

            await _authService.LogoutAsync(userUuid);
            return Ok("Successful logout");
        }
    }
}
