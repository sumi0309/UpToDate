using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Up_To_Date__UTD_.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

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

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    
    string adminEmail = "sumi0309@umd.edu";
    string editorEmail = "sumiranjaiswal09@gmail.com";
    
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    var editorUser = await userManager.FindByEmailAsync(editorEmail);
   
    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
    if (!await userManager.IsInRoleAsync(editorUser, "Editor"))
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

app.Run();
