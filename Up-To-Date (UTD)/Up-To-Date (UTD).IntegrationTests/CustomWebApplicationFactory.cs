using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Up_To_Date__UTD_.Data;
using Up_To_Date__UTD_.Models;


namespace Up_To_Date__UTD_.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the app's ApplicationDbContext registration.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add ApplicationDbContext using an in-memory database for testing.
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database context (ApplicationDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();
                    SeedData(db);

                    // Seed the database with test data (optional).
                    // SeedData.PopulateTestData(db);
                }
            });
        }
        private void SeedData(ApplicationDbContext context)
        {
            // Seed data for complex scenarios
            context.Suggestions.AddRange(
                new Suggestion { Content = "First Test Suggestion", DatePosted = DateTime.Now },
                new Suggestion { Content = "Second Test Suggestion", DatePosted = DateTime.Now.AddDays(-1) }
            );

            context.SaveChanges();
        }
    }
}
