using System.Threading.Tasks;

namespace Jade.Services
{
    public interface ILikeService
    {
        Task<bool> LikePostAsync(int postId, string userId);
        Task<bool> UnlikePostAsync(int postId, string userId);
    }
}
