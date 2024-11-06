using System.Collections.Generic;
using System.Threading.Tasks;
using Jade.DTO;

namespace Jade.Services
{
    public interface IPostService
    {
        Task<PostDto> CreatePostAsync(CreatePostDto createPostDto, string userId);
        Task<PostDto> GetPostByIdAsync(int postId);
        Task<IEnumerable<PostDto>> GetAllPublicPostsAsync();
        Task<IEnumerable<PostDto>> GetUserPostsAsync(string userId);
        Task<bool> DeletePostAsync(int postId, string userId);
    }
}
