using AuthenticationMicroservice.Application.Common.Interfaces;
using AuthenticationMicroservice.Application.DTOs.Requests;
using AuthenticationMicroservice.Application.DTOs.Responses;
using AuthenticationMicroservice.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AuthenticationMicroservice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("auth-policy")]
    public class AuthController(IAuthService authService, ICurrentUserService currentUserService) : ControllerBase
    {
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserRegisteredResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserRegisteredResponse>> RegisterUserAsync([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var response = await authService.RegisterAsync(request, ct);

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthTokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthTokenResponse>> LoginAsync([FromBody] LoginRequest request, CancellationToken ct)
        {
            var ipAddress = GetIpAddress();

            var response = await authService.LoginAsync(request, ipAddress, ct);

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthTokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct)
        {
            var ipAddress = GetIpAddress();

            var response = await authService.RefreshTokenAsync(request, ipAddress, ct);

            return Ok(response);
        }

        [HttpPost("revoke-token")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RevokeToken([FromBody] RevokeTokenRequest request, CancellationToken ct)
        {
            var ipAddress = GetIpAddress();

            await authService.RevokeTokenAsync(request, ipAddress, ct);

            return NoContent();
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserProfileResponse>> GetMe(CancellationToken ct)
        {
            var response = await authService.GetCurrentUserProfileAsync(currentUserService.UserId, ct);

            return Ok(response);
        }

        private string? GetIpAddress() => HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
