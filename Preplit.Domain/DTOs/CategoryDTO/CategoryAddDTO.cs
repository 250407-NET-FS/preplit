using System.ComponentModel.DataAnnotations;

namespace Preplit.Domain.DTOs
{
    public class CategoryAddDTO
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public Guid UserId { get; set; }
    }
}