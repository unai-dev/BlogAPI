using AutoMapper;
using BlogAPI.Data;
using BlogAPI.DTOs.Post;
using BlogAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Controllers
{
    [ApiController]
    [Route("v1/api/users/{userId:string}/posts")]
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public PostController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<PostDTO>> Get(string userId)
        {
            var posts = await context.Posts
                .Include(u => u.User)
                .Where(x => x.UserId == userId)
                .ToListAsync();
            var mapPosts = mapper.Map<IEnumerable<PostDTO>>(posts);
            return mapPosts;
        }

        [HttpGet("{id}", Name ="GetPost")]
        public async Task<ActionResult<PostDTO>> Get(Guid id)
        {
            var post = await context.Posts.FirstOrDefaultAsync(x => x.Id == id);

            if (post is null) return NotFound();

            return mapper.Map<PostDTO>(post);
        }

        [HttpPost]
        public async Task<ActionResult> Post(string userId, AddPostDTO addPostDTO)
        {
            var post = mapper.Map<Post>(addPostDTO);
            context.Add(post);
            await context.SaveChangesAsync();

            var postCreated = mapper.Map<PostDTO>(post);
            return CreatedAtRoute("GetPost", new { id = post.Id }, postCreated);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string userId, Guid id)
        {
            var post = await context.Posts.Where(x => x.Id == id).ExecuteDeleteAsync();

            if (post == 0) return NotFound();

            return NoContent();
        }
    }
}
