using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Preplit.Domain;

namespace Preplit.Domain {
    [Table("Cards")]
    public class Card
    {
        [Key]
        public Guid CardId { get; set; } = Guid.NewGuid();
        [Required]
        public string? Question { get; set; }
        [Required]
        public string? Answer { get; set; }
        [ForeignKey(nameof(Category))]
        public Guid? CategoryId { get; set; }
        [ForeignKey(nameof(User))]
        [Required]
        public Guid UserId { get; set; }

        #nullable disable
        public User User { get; set; }
        public Category Category { get; set; }
        #nullable restore

        public Card(string question, string answer, Guid? categoryId, Guid userId)
        {
            Question = question;
            Answer = answer;
            CategoryId = categoryId;
            UserId = userId;
        }
        
        public Card() {}
    }
}