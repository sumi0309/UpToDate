using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

public class ProgramUnitTests
{
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;

    public ProgramUnitTests()
    {
        // Mock RoleManager
        var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            roleStoreMock.Object, null, null, null, null);

        // Mock UserManager
        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task EnsureRolesCreated_WhenNotExist()
    {
        string adminRole = "Admin";
        string editorRole = "Editor";

        _roleManagerMock.Setup(r => r.RoleExistsAsync(adminRole)).ReturnsAsync(false);
        _roleManagerMock.Setup(r => r.RoleExistsAsync(editorRole)).ReturnsAsync(false);
        _roleManagerMock.Setup(r => r.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

        if (!await _roleManagerMock.Object.RoleExistsAsync(adminRole))
        {
            await _roleManagerMock.Object.CreateAsync(new IdentityRole(adminRole));
        }

        if (!await _roleManagerMock.Object.RoleExistsAsync(editorRole))
        {
            await _roleManagerMock.Object.CreateAsync(new IdentityRole(editorRole));
        }

        // Assert
        _roleManagerMock.Verify(r => r.CreateAsync(It.Is<IdentityRole>(role => role.Name == adminRole)), Times.Once);
        _roleManagerMock.Verify(r => r.CreateAsync(It.Is<IdentityRole>(role => role.Name == editorRole)), Times.Once);
    }

    [Fact]
    public async Task EnsureUsersAssignedToRoles()
    {
        string adminEmail = "sumi0309@umd.edu";
        string editorEmail = "sumiranjaiswal09@gmail.com";

        var adminUser = new IdentityUser { Email = adminEmail };
        var editorUser = new IdentityUser { Email = editorEmail };

        _userManagerMock.Setup(u => u.FindByEmailAsync(adminEmail)).ReturnsAsync(adminUser);
        _userManagerMock.Setup(u => u.FindByEmailAsync(editorEmail)).ReturnsAsync(editorUser);
        _userManagerMock.Setup(u => u.IsInRoleAsync(adminUser, "Admin")).ReturnsAsync(false);
        _userManagerMock.Setup(u => u.IsInRoleAsync(editorUser, "Editor")).ReturnsAsync(false);
        _userManagerMock.Setup(u => u.AddToRoleAsync(adminUser, "Admin")).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(u => u.AddToRoleAsync(editorUser, "Editor")).ReturnsAsync(IdentityResult.Success);

        if (adminUser != null && !await _userManagerMock.Object.IsInRoleAsync(adminUser, "Admin"))
        {
            await _userManagerMock.Object.AddToRoleAsync(adminUser, "Admin");
        }

        if (editorUser != null && !await _userManagerMock.Object.IsInRoleAsync(editorUser, "Editor"))
        {
            await _userManagerMock.Object.AddToRoleAsync(editorUser, "Editor");
        }

        _userManagerMock.Verify(u => u.AddToRoleAsync(adminUser, "Admin"), Times.Once);
        _userManagerMock.Verify(u => u.AddToRoleAsync(editorUser, "Editor"), Times.Once);
    }
}
