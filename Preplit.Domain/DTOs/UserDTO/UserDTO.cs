using System.ComponentModel.DataAnnotations;

namespace Preplit.Domain.DTOs
{

    public class UserDTO
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Email { get; set; }
    }
}