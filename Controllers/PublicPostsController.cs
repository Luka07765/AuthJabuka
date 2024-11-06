using Jade.DTO;
using Jade.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jade.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicPostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PublicPostsController(IPostService postService)
        {
            _postService = postService;
        }

        // GET: api/PublicPosts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPublicPosts()
        {
            var posts = await _postService.GetAllPublicPostsAsync();
            return Ok(posts);
        }

        // GET: api/PublicPosts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPost(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
                return NotFound();

            return Ok(post);
        }
    }
}
