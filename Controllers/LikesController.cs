using Jade.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Jade.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LikesController : ControllerBase
    {
        private readonly ILikeService _likeService;

        public LikesController(ILikeService likeService)
        {
            _likeService = likeService;
        }

        // POST: api/Likes/Like/5
        [HttpPost("Like/{postId}")]
        public async Task<IActionResult> LikePost(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _likeService.LikePostAsync(postId, userId);
            if (!result)
                return BadRequest("You have already liked this post.");

            return Ok("Post liked successfully.");
        }

        // POST: api/Likes/Unlike/5
        [HttpPost("Unlike/{postId}")]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _likeService.UnlikePostAsync(postId, userId);
            if (!result)
                return BadRequest("You have not liked this post yet.");

            return Ok("Post unliked successfully.");
        }
    }
}
