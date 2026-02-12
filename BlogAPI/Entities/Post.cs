using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Entities
{
    public class Post
    {
        public Guid Id { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(450)]
        public required string Title { get; set; }
        public required string UserId { get; set; }
        public IdentityUser? User { get; set; }
    }
}
