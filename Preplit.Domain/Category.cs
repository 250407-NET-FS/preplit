using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Preplit.Domain {
    [Table("Categories")]
    public class Category(string name, Guid userId)
    {
        [Key]
        public Guid CategoryId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = name;

        [Required]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; } = userId;

        #nullable disable
        public User User { get; set; }
        #nullable restore
    }
}