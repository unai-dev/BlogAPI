using BlogAPI.DTOs.Post;

namespace BlogAPI.DTOs.User
{
    public class UserDTO
    {
        public required string Email { get; set; }
        public DateTime Birthdate { get; set; }
        public List<PostDTO> Posts { get; set; } = [];
    }
}
