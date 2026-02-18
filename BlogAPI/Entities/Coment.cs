using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Entities
{
    public class Coment
    {
        public Guid Id { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(1000)]
        public required string Description { get; set; }

        public required string UserId { get; set; }
        public User? User {get; set;}
        public Guid PostId { get; set; }
        public Post? Post { get; set; }
    }
}
