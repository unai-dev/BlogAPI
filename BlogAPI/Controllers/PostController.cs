using AutoMapper;
using BlogAPI.Data;
using BlogAPI.DTOs.Post;
using BlogAPI.Entities;
using BlogAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Controllers
{
    [ApiController]
    [Route("v1/api/users/{userId}/posts")]
    [Authorize(Policy = "admin")]
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IUserService userService;

        public PostController(ApplicationDbContext context, IMapper mapper, IUserService userService)
        {
            this.context = context;
            this.mapper = mapper;
            this.userService = userService;
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
        public async Task<ActionResult<PostDTO>> Get(Guid id, string userId)
        {
            var post = await context.Posts.Include(u => u.User).Where(x => x.UserId == userId).FirstOrDefaultAsync(x => x.Id == id);

            if (post is null) return NotFound();

            return mapper.Map<PostDTO>(post);
        }

        [HttpPost]
        public async Task<ActionResult> Post(string userId, AddPostDTO addPostDTO)
        {

            var user = await userService.GetUser();

            if (user is null) return NotFound();

            if (user.Id != userId) return Forbid();

            var post = mapper.Map<Post>(addPostDTO);
            post.UserId = userId;
            context.Add(post);
            await context.SaveChangesAsync();

            var postCreated = mapper.Map<PostDTO>(post);
            return CreatedAtRoute("GetPost", new { id = post.Id, userId = post.UserId }, postCreated);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string userId, Guid id)
        {
            var user = await userService.GetUser();

            if (user is null) return NotFound();

            if (user.Id != userId) return Forbid();

            var post = await context.Posts.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (post is null) return NotFound();

            context.Remove(post);
            await context.SaveChangesAsync();

            return NoContent();
        }

    }
}
