using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Preplit.Domain {
    [Table("Categories")]
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; } = Guid.NewGuid();
        [Required]
        public string? Name { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        #nullable disable
        public User User { get; set; }
        #nullable restore

        public Category(string name, Guid userId)
        {
            Name = name;
            UserId = userId;
        }
        
        public Category() { }
    }
}