using System.ComponentModel.DataAnnotations;

namespace BlogAPI.DTOs.Coment
{
    public class AddComentDTO
    {
        [Required]
        [MinLength(10)]
        [MaxLength(1000)]
        public required string Description { get; set; }

        public int UserId { get; set; }
        public int PostId { get; set; }
    }
}
