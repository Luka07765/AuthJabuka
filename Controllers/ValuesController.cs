using Jade.DTO;
using Jade.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Jade.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserPostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public UserPostsController(IPostService postService)
        {
            _postService = postService;
        }

        // GET: api/UserPosts
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetUserPosts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var posts = await _postService.GetUserPostsAsync(userId);
            return Ok(posts);
        }

        // POST: api/UserPosts
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto createPostDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = await _postService.CreatePostAsync(createPostDto, userId);
            return CreatedAtAction(nameof(GetUserPosts), new { id = post.Id }, post);
        }

        // DELETE: api/UserPosts/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _postService.DeletePostAsync(id, userId);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
