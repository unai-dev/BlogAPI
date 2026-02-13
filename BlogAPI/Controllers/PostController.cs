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
    [Route("v1/api/posts")]
    [Authorize]
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
        [AllowAnonymous]
        public async Task<IEnumerable<PostDTO>> Get()
        {
            var posts = await context.Posts.Include(u => u.User).AsNoTracking().ToListAsync();
            var mapPosts = mapper.Map<IEnumerable<PostDTO>>(posts);
            return mapPosts;
        }

        [HttpGet("{id}", Name ="GetPost")]
        public async Task<ActionResult<PostDTO>> Get(Guid id)
        {
            var post = await context.Posts.Include(u => u.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (post is null) return NotFound();

            return Ok(mapper.Map<PostDTO>(post));
        }

        [HttpPost]
        public async Task<ActionResult> Post(AddPostDTO addPostDTO)
        {

            var user = await userService.GetUser();

            if (user is null) return Unauthorized();

            var post = mapper.Map<Post>(addPostDTO);

            post.UserId = user.Id;
            context.Add(post);
            await context.SaveChangesAsync();

            var postCreated = mapper.Map<PostDTO>(post);
            return CreatedAtRoute("GetPost", new { id = post.Id}, postCreated);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(Guid id, AddPostDTO postDTO)
        {
            var postDB = await context.Posts.FirstOrDefaultAsync(x => x.Id == id);

            if (postDB is null) return NotFound();

            var post = mapper.Map<Post>(postDTO);
            post.Id = id;

            context.Update(post);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var user = await userService.GetUser();

            if (user is null) return NotFound();

            var post = await context.Posts.FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);

            if (post is null) return NotFound();

            context.Remove(post);
            await context.SaveChangesAsync();

            return NoContent();
        }

    }
}
