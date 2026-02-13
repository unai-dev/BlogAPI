using BlogAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace BlogAPI.DTOs.Post
{
    public class AddPostDTO
    {
        [Required]
        [MinLength(10)]
        [MaxLength(450)]
        [FirstUpperCaseValidation]
        public required string Title { get; set; }
    }
}
