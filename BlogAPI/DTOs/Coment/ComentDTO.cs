using System.ComponentModel.DataAnnotations;

namespace BlogAPI.DTOs.Coment
{
    public class ComentDTO
    {
        public Guid Id { get; set; }
        public required string Description { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
    }
}
