using System.ComponentModel.DataAnnotations;

namespace BlogAPI.DTOs.Claims
{
    public class EditClaimDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
