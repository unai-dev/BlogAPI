using Microsoft.AspNetCore.Identity;

namespace BlogAPI.DTOs.Post
{
    public class PostDTO
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string UserId { get; set; }
        public required string UserEmail { get; set; }
    }
}
