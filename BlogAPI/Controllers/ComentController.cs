using AutoMapper;
using BlogAPI.Data;
using BlogAPI.DTOs.Coment;
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

        public ComentController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<ComentDTO>> Get(Guid postId)
        {
            var coments = await context.Coments.Where(x => x.PostId == postId).ToListAsync();
            return mapper.Map<IEnumerable<ComentDTO>>(coments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ComentDTO>> Get(Guid id, Guid postId)
        {
            var post = await context.Posts.FirstOrDefaultAsync(x => x.Id == postId);

            if (post is null)
            {
                return NotFound();
            }

            var coment = await context.Coments.FirstOrDefaultAsync(x => x.Id == id);
            if (coment is null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<ComentDTO>(coment));
        }


    }
}
