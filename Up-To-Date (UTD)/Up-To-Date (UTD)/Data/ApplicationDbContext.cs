using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serilog; 
using Up_To_Date__UTD_.Models;

namespace Up_To_Date__UTD_.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        private readonly Serilog.ILogger _logger;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            _logger = Log.ForContext<ApplicationDbContext>(); 
        }

        // DbSet for managing News items in the database.
        public DbSet<Up_To_Date__UTD_.Models.News> News { get; set; } = default!;

        // DbSet for managing Suggestions in the database.
        public DbSet<Suggestion> Suggestions { get; set; }

        // DbSet for managing Articles in the database.
        public DbSet<Article> Articles { get; set; }

        // Overriding SaveChangesAsync to log database actions.
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    _logger.Information("Adding new entity: {EntityName}, Data: {@Entity}", entry.Entity.GetType().Name, entry.Entity);
                }
                else if (entry.State == EntityState.Modified)
                {
                    _logger.Information("Modifying entity: {EntityName}, Data: {@Entity}", entry.Entity.GetType().Name, entry.Entity);
                }
            }

            try
            {
                var result = await base.SaveChangesAsync(cancellationToken);
                _logger.Information("Database changes saved successfully.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while saving database changes.");
                throw;
            }
        }

        // Overriding SaveChanges to log synchronous database actions (if needed).
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    _logger.Information("Adding new entity: {EntityName}, Data: {@Entity}", entry.Entity.GetType().Name, entry.Entity);
                }
                else if (entry.State == EntityState.Modified)
                {
                    _logger.Information("Modifying entity: {EntityName}, Data: {@Entity}", entry.Entity.GetType().Name, entry.Entity);
                }
            }

            try
            {
                var result = base.SaveChanges();
                _logger.Information("Database changes saved successfully.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while saving database changes.");
                throw;
            }
        }
    }
}
