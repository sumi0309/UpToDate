using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Up_To_Date__UTD_.Data;
using Xunit;

public class ProgramIntegrationTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ApplicationDbContext _context;

    public ProgramIntegrationTests()
    {
        var services = new ServiceCollection();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        _serviceProvider = services.BuildServiceProvider();

        _context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
    }

    [Fact]
    public async Task CreateRoles_And_AssignUsersToRoles_Test()
    {
        var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string adminRole = "Admin";
        string editorRole = "Editor";

        string adminEmail = "sumi0309@umd.edu";
        string editorEmail = "sumiranjaiswal09@gmail.com";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }
        if (!await roleManager.RoleExistsAsync(editorRole))
        {
            await roleManager.CreateAsync(new IdentityRole(editorRole));
        }

        Assert.True(await roleManager.RoleExistsAsync(adminRole));
        Assert.True(await roleManager.RoleExistsAsync(editorRole));

        var adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
        var editorUser = new IdentityUser { UserName = editorEmail, Email = editorEmail };

        await userManager.CreateAsync(adminUser, "Test@123");
        await userManager.CreateAsync(editorUser, "Test@123");

        await userManager.AddToRoleAsync(adminUser, adminRole);
        await userManager.AddToRoleAsync(editorUser, editorRole);

        Assert.True(await userManager.IsInRoleAsync(adminUser, adminRole));
        Assert.True(await userManager.IsInRoleAsync(editorUser, editorRole));
    }

    public void Dispose()
    {
        _serviceProvider?.Dispose();
        _context?.Database.EnsureDeleted(); 
    }
}
