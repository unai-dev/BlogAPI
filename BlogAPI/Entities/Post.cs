using BlogAPI.Validations;
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
        [FirstUpperCaseValidation]
        public required string Title { get; set; }
        public required string UserId { get; set; }
        public User? User { get; set; }
        public List<Coment> Coments { get; set; } = [];
    }
}
