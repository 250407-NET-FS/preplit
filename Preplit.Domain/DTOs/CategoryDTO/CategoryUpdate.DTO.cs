using System.ComponentModel.DataAnnotations;

namespace Preplit.Domain.DTOs
{
    public class CategoryUpdateDTO
    {
        [Required]
        public Guid CategoryId { get; set; }
        public string? Question { get; set; } = null!;
        public string? Answer { get; set; }
        public Guid? UserId { get; set; }
    }
}