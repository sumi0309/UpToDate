using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Up_To_Date__UTD_.Models;

namespace Up_To_Date__UTD_.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Up_To_Date__UTD_.Models.News> News { get; set; } = default!;
        public DbSet<Suggestion> Suggestions { get; set; }

        public DbSet<Article> Articles { get; set; }
    }
}
