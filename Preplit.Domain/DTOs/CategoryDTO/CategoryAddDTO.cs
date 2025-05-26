using System.ComponentModel.DataAnnotations;

namespace Preplit.Domain.DTOs
{
    public class CategoryAddDTO
    {
        
        public string? Name { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }
}