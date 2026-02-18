using AutoMapper;
using BlogAPI.Data;
using BlogAPI.DTOs.Coment;
using BlogAPI.DTOs.Post;
using BlogAPI.Entities;
using BlogAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Controllers
{
    [ApiController]
    [Route("v1/api/posts/{postId}/coments")]
    [Authorize]
    public class ComentController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IUserService userService;

        public ComentController(ApplicationDbContext context, IMapper mapper, IUserService userService)
        {
            this.context = context;
            this.mapper = mapper;
            this.userService = userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<ComentDTO>> Get(Guid postId)
        {
            var coments = await context.Coments.Where(x => x.PostId == postId)
                .Include(p => p.Post).ToListAsync();
            return mapper.Map<IEnumerable<ComentDTO>>(coments);
        }

        [HttpGet("{id}", Name ="GetComent")]
        public async Task<ActionResult<ComentDTO>> Get(Guid id, Guid postId)
        {
            var post = await context.Posts.FirstOrDefaultAsync(x => x.Id == postId);

            if (post is null)
            {
                return NotFound();
            }

            var coment = await context.Coments.Include(p => p.Post)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (coment is null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<ComentDTO>(coment));
        }


        [HttpPost]
        public async Task<ActionResult> Post(AddComentDTO addComentDTO, Guid postId)
        {
            var post = await context.Posts.AnyAsync(x => x.Id == postId);
            if (!post) return NotFound();

            var user = await userService.GetUser();

            if (user is null)
            {
                return NotFound();
            }

            var coment = mapper.Map<Coment>(addComentDTO);
            coment.PostId = postId;
            coment.UserId = user.Id;
            context.Add(coment);
            await context.SaveChangesAsync();

            var comentDTO = mapper.Map<ComentDTO>(coment);

            return CreatedAtRoute("GetComent", new { id = coment.Id }, comentDTO);


        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid postId, Guid id)
        {
            var post = await context.Posts.FirstOrDefaultAsync(x => x.Id == postId);
            
            if (post is null)
            {
                return NotFound();
            }

            var coment = await context.Coments.Where(x => x.Id == id).ExecuteDeleteAsync();

            if (coment == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
