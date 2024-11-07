using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Jade.DTO;
using Jade.Services;
using Jade.Models;
using Microsoft.AspNetCore.Identity;

namespace Jade.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(
            ITokenService tokenService,
            IUserService userService,
            IRefreshTokenService refreshTokenService,
            UserManager<IdentityUser> userManager)
        {
            _tokenService = tokenService;
            _userService = userService;
            _refreshTokenService = refreshTokenService;
            _userManager = userManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.RegisterUser(model);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(ModelState);
            }

            // Assign default role
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "User"); // Default role: User
            }

            return Ok("User registered successfully with default role!");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.AuthenticateUser(model);
            if (user == null)
                return Unauthorized("Invalid login attempt.");

            var ipAddress = GetIpAddress();

            var tokenResponse = await _tokenService.CreateTokenResponse(user, ipAddress);

            return Ok(tokenResponse);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
        {
            var ipAddress = GetIpAddress();

            // Retrieve the existing refresh token
            var existingToken = await _refreshTokenService.GetRefreshToken(model.RefreshToken);

            // Check if the refresh token is valid
            if (existingToken == null || !existingToken.IsActive)
                return Unauthorized("Invalid or expired refresh token.");

            // Get the user associated with the token
            var user = existingToken.User;

            // Invalidate the old refresh token and generate a new one
            var newRefreshToken = await _refreshTokenService.GenerateRefreshToken(user.Id, ipAddress);
            await _refreshTokenService.InvalidateRefreshToken(existingToken, ipAddress, newRefreshToken.Token);

            // Generate a new access token with updated roles
            // In AuthController
            var newAccessToken = await _tokenService.CreateAccessToken(user);


            // Return the new access and refresh tokens
            var tokenResponse = new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };

            return Ok(tokenResponse);
        }


        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ipAddress = GetIpAddress();

            await _refreshTokenService.InvalidateAllUserRefreshTokens(userId, ipAddress);

            return Ok("Logged out successfully.");
        }

        // Helper method to get IP address
        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
