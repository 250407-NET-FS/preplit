using System.ComponentModel.DataAnnotations;

namespace Preplit.Domain.DTOs
{
    public class CardAddDTO
    {
        [Required]
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public Guid? CategoryId { get; set; }
        [Required]
        public Guid OwnerId { get; set; }
    }
}