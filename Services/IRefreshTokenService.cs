using Jade.Models;

namespace Jade.Services
{
    
    public interface IRefreshTokenService
    {
        Task<RefreshToken> GenerateRefreshToken(string userId);
        Task<RefreshToken> GetRefreshToken(string token);
        Task InvalidateRefreshToken(RefreshToken token);
    }
}

