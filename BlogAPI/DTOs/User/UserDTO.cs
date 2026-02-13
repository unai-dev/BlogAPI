using BlogAPI.DTOs.Post;

namespace BlogAPI.DTOs.User
{
    public class UserDTO
    {
        public required string Email { get; set; }
        public DateTime Birthday { get; set; }
        public List<PostDTO> Posts { get; set; } = [];
    }
}
