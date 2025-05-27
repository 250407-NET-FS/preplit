using System.ComponentModel.DataAnnotations;

namespace Preplit.Domain.DTOs
{
    public class CardUpdateDTO
    {
        [Required]
        public Guid CardId { get; set; }
        public string? Question { get; set; } = null!;
        public string Answer { get; set; } = null!;
        public Guid? CategoryId { get; set; }
        public Guid? OwnerId { get; set; }
    }
}