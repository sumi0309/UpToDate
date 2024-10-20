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
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                    db.Database.EnsureCreated();
                    SeedData(db); 
                }
            });
        }

        // Method to seed the database with initial test data.
        private void SeedData(ApplicationDbContext context)
        {
            context.Suggestions.AddRange(
                new Suggestion { Content = "First Test Suggestion", DatePosted = DateTime.Now },
                new Suggestion { Content = "Second Test Suggestion", DatePosted = DateTime.Now.AddDays(-1) }
            );

            context.SaveChanges(); 
        }
    }
}
