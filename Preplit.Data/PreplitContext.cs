using Microsoft.EntityFrameworkCore;
using Preplit.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Preplit.Data {
    public class PreplitContext(DbContextOptions<PreplitContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
    {
        public DbSet<Card> Cards { get; set; }
        public DbSet<Category> Categories { get; set; } 
    }
}