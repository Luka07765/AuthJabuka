using System.Threading.Tasks;
using Jade.Data;
using Jade.Models;
using Microsoft.EntityFrameworkCore;

namespace Jade.Services
{
    public class LikeService : ILikeService
    {
        private readonly ApplicationDbContext _context;

        public LikeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> LikePostAsync(int postId, string userId)
        {
            // Check if the user already liked the post
            var existingLike = await _context.Likes
                .SingleOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (existingLike != null)
                return false; // Already liked

            var like = new Like
            {
                PostId = postId,
                UserId = userId
            };

            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnlikePostAsync(int postId, string userId)
        {
            var like = await _context.Likes
                .SingleOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (like == null)
                return false; // Not liked yet

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
