using System.ComponentModel.DataAnnotations;

namespace Preplit.Domain.DTOs
{
    public class CategoryResponseDTO(Category category)
    {
        public Guid CategoryId { get; set; } = category.CategoryId;
        public string? Name { get; set; } = category.Name;

        [Required]
        public Guid UserId { get; set; } = category.UserId;
    }
}