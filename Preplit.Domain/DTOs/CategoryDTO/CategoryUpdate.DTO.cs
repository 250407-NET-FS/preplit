using System.ComponentModel.DataAnnotations;

namespace Preplit.Domain.DTOs
{
    public class CategoryUpdateDTO
    {
        [Required]
        public Guid CategoryId { get; set; }
        public string? Name { get; set; } = null!;
        public Guid? UserId { get; set; }
    }
}