using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Jade.DTO;
using Jade.Services;
using Jade.Models;

namespace Jade.Controllers // Adjusted to match your project's namespace
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthController(
            ITokenService tokenService,
            IUserService userService,
            IRefreshTokenService refreshTokenService)
        {
            _tokenService = tokenService;
            _userService = userService;
            _refreshTokenService = refreshTokenService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.RegisterUser(model);
            if (result.Succeeded)
                return Ok("User registered successfully!");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.AuthenticateUser(model);
            if (user == null)
                return Unauthorized("Invalid login attempt.");

            // Generate access token and refresh token
            var tokenResponse = await _tokenService.CreateTokenResponse(user);

            return Ok(tokenResponse);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingToken = await _refreshTokenService.GetRefreshToken(model.RefreshToken);

            if (existingToken == null || existingToken.IsExpired)
                return Unauthorized("Invalid or expired refresh token.");

            var user = existingToken.User;

            // Optionally, check if the user is still active

            var newTokenResponse = await _tokenService.CreateTokenResponse(user);

            // Invalidate the old refresh token
            await _refreshTokenService.InvalidateRefreshToken(existingToken);

            return Ok(newTokenResponse);
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var refreshToken = await _refreshTokenService.GetRefreshToken(model.RefreshToken);

            if (refreshToken != null && refreshToken.UserId == userId)
            {
                await _refreshTokenService.InvalidateRefreshToken(refreshToken);
                return Ok("Logged out successfully.");
            }

            return BadRequest("Invalid refresh token.");
        }
    }
}
