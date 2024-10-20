using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Up_To_Date__UTD_.Models;

namespace Up_To_Date__UTD_.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        // Constructor to initialize the DbContext with options.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet for managing News items in the database.
        public DbSet<Up_To_Date__UTD_.Models.News> News { get; set; } = default!;

        // DbSet for managing Suggestions in the database.
        public DbSet<Suggestion> Suggestions { get; set; }

        // DbSet for managing Articles in the database.
        public DbSet<Article> Articles { get; set; }
    }
}
