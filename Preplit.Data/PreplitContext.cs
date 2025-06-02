using Microsoft.EntityFrameworkCore;
using Preplit.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Preplit.Data {
    public class PreplitContext(DbContextOptions<PreplitContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
    {
        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<Category> Categories { get; set; } 
    }
}