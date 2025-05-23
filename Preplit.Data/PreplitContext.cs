using Microsoft.EntityFrameworkCore;
using Preplit.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Preplit.Data {
    public class PreplitContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public PreplitContext(DbContextOptions<PreplitContext> options) : base(options) { }

        public DbSet<Card> Cards { get; set; }
        public DbSet <Category> Categories { get; set; } 
    }
}