using System.ComponentModel.DataAnnotations;

namespace Preplit.Domain.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}