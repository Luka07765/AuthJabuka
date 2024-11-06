using Microsoft.AspNetCore.Identity;
using Jade.Models;
namespace Jade.Services
{
    public interface ITokenService
    {
        Task<string> CreateAccessToken(IdentityUser user);
        Task<TokenResponse> CreateTokenResponse(IdentityUser user, string ipAddress);
    }
}
