using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Entities
{
    public class User: IdentityUser
    {
        public DateTime Birthdate { get; set; }
        public List<Post> Posts { get; set; } = [];
        public List<Coment> Coments { get; set; } = [];
    }
}
