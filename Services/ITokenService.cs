using Microsoft.AspNetCore.Identity;

namespace Jade.Services
{
    public interface ITokenService
    {
        Task<string> CreateToken(IdentityUser user);
    }
}
