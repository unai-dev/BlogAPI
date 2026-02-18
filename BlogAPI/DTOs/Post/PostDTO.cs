using BlogAPI.DTOs.Coment;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.DTOs.Post
{
    public class PostDTO
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string UserEmail { get; set; }
        public List<ComentDTO> Coments { get; set; } = [];
    }
}
