using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Preplit.Domain;

namespace Preplit.Domain {
    [Table("Cards")]
    public class Card(string question, string answer, Guid? categoryId, Guid userId)
    {
        [Key]
        public Guid CardId { get; set; } = Guid.NewGuid();
        [Required]
        public string Question { get; set; } = question;
        [Required]
        public string Answer { get; set; } = answer;
        [ForeignKey(nameof(Category))]
        public Guid? CategoryId { get; set; } = categoryId;
        [ForeignKey(nameof(User))]
        [Required]
        public Guid UserId { get; set; } = userId;

#nullable disable
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public User User { get; set; }
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public Category Category { get; set; }
        #nullable restore

    }
}