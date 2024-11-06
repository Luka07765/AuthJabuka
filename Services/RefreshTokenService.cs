using Jade.Data;
using Jade.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Jade.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GenerateRefreshToken(string userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = GenerateTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                UserId = userId
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<RefreshToken> GetRefreshToken(string token)
        {
            return await _context.RefreshTokens.Include(rt => rt.User)
                .SingleOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task InvalidateRefreshToken(RefreshToken token)
        {
            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();
        }

        private string GenerateTokenString()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
