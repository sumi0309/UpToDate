using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Up_To_Date__UTD_.Data;

namespace Up_To_Date__UTD_
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console() 
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) 
                .CreateLogger();

            try
            {
                Log.Information("Starting the application");

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

                // Creating admin and editor roles
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
                    Log.Information("Authorization roles created.");
                }

                if (app.Environment.IsDevelopment())
                {
                    app.UseMigrationsEndPoint();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                // Assigning admin and editor role to two users
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
                    Log.Information("Roles assigned.");
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.MapRazorPages();

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
