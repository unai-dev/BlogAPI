using AutoMapper;
using BlogAPI.Data;
using BlogAPI.DTOs.Post;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers
{
    [ApiController]
    [Route("v1/api/users/{userId}/posts")]
    public class PostController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public PostController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<PostDTO>>> Get(int userId)
        {
            var user = await context.
        }
    }
}
