using Jade.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jade.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly UserManager<IdentityUser> _userManager;

        public TokenService(IConfiguration configuration, IRefreshTokenService refreshTokenService, UserManager<IdentityUser> userManager)
        {
            _configuration = configuration;
            _refreshTokenService = refreshTokenService;
            _userManager = userManager;
        }

        public async Task<string> CreateAccessToken(IdentityUser user)
        {
            // Initialize authClaims as a List to allow dynamic additions
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            // Add each role as a claim
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddMinutes(15), // Access token expiration
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<TokenResponse> CreateTokenResponse(IdentityUser user, string ipAddress)
        {
            var accessToken = await CreateAccessToken(user);
            var refreshToken = await _refreshTokenService.GenerateRefreshToken(user.Id, ipAddress);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }
    }
}
