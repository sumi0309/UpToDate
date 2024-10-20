using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Up_To_Date__UTD_.Data;

namespace Up_To_Date__UTD_  // Ensure the namespace matches your project
{
    public class Program
    {
        public static async Task Main(string[] args) // Changed to async Task
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Ensure roles are created
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string adminRole = "Admin";
                string editorRole = "Editor";

                if (!await roleManager.RoleExistsAsync(adminRole))
                {
                    var role = new IdentityRole(adminRole);
                    role.NormalizedName = adminRole.ToUpper();
                    await roleManager.CreateAsync(role);
                }

                if (!await roleManager.RoleExistsAsync(editorRole))
                {
                    var role = new IdentityRole(editorRole);
                    role.NormalizedName = editorRole.ToUpper();
                    await roleManager.CreateAsync(role);
                }
            }

            // Handle error pages
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Ensure users are assigned to roles
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                string adminEmail = "sumi0309@umd.edu";
                string editorEmail = "sumiranjaiswal09@gmail.com";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                var editorUser = await userManager.FindByEmailAsync(editorEmail);

                if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                if (editorUser != null && !await userManager.IsInRoleAsync(editorUser, "Editor"))
                {
                    await userManager.AddToRoleAsync(editorUser, "Editor");
                }
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            await app.RunAsync(); // Ensure to run asynchronously
        }
    }
}
