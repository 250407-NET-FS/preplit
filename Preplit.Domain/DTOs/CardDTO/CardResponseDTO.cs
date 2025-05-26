using System.ComponentModel.DataAnnotations;

namespace Preplit.Domain.DTOs
{
    public class CardResponseDTO(Card card)
    {
        public Guid CardId { get; set; } = card.CardId;
        public string Question { get; set; } = card.Question;
        public string? Answer { get; set; } = card.Answer;
        public Guid? CategoryId { get; set; } = card.CategoryId;
    }
}