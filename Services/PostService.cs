using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jade.Data;
using Jade.DTO;
using Jade.Models;
using Microsoft.EntityFrameworkCore;

namespace Jade.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PostDto> CreatePostAsync(CreatePostDto createPostDto, string userId)
        {
            var post = new Post
            {
                Title = createPostDto.Title,
                Content = createPostDto.Content,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId
            };

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            return await GetPostByIdAsync(post.Id);
        }

        public async Task<PostDto> GetPostByIdAsync(int postId)
        {
            var post = await _context.Posts
                .Include(p => p.CreatedByUser)
                .Include(p => p.Likes)
                .SingleOrDefaultAsync(p => p.Id == postId);

            if (post == null) return null;

            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedByUserName = post.CreatedByUser.UserName,
                CreatedAt = post.CreatedAt,
                LikeCount = post.Likes.Count
            };
        }

        public async Task<IEnumerable<PostDto>> GetAllPublicPostsAsync()
        {
            var posts = await _context.Posts
                .Include(p => p.CreatedByUser)
                .Include(p => p.Likes)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return posts.Select(post => new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedByUserName = post.CreatedByUser.UserName,
                CreatedAt = post.CreatedAt,
                LikeCount = post.Likes.Count
            });
        }

        public async Task<IEnumerable<PostDto>> GetUserPostsAsync(string userId)
        {
            var posts = await _context.Posts
                .Include(p => p.Likes)
                .Where(p => p.CreatedByUserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return posts.Select(post => new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                LikeCount = post.Likes.Count
            });
        }

        public async Task<bool> DeletePostAsync(int postId, string userId)
        {
            var post = await _context.Posts.SingleOrDefaultAsync(p => p.Id == postId && p.CreatedByUserId == userId);
            if (post == null) return false;

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
