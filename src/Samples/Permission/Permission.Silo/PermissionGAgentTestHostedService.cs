using Aevatar.Core.Abstractions.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace Permission.Silo;

public class PermissionGAgentTestHostedService : IHostedService
{
    private readonly IAbpApplicationWithExternalServiceProvider _application;
    private readonly IServiceProvider _serviceProvider;

    public PermissionGAgentTestHostedService(
        IAbpApplicationWithExternalServiceProvider application,
        IServiceProvider serviceProvider)
    {
        _application = application;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _application.InitializeAsync(_serviceProvider);
        // create role
        var roleManager = _serviceProvider.GetRequiredService<IdentityRoleManager>();
        await roleManager.CreateAsync(new IdentityRole(Guid.NewGuid(), "Owner"));
        await roleManager.CreateAsync(new IdentityRole(Guid.NewGuid(), "Master"));
        await roleManager.CreateAsync(new IdentityRole(Guid.NewGuid(), "Developer"));
        await roleManager.CreateAsync(new IdentityRole(Guid.NewGuid(), "Guest"));

        // grant permission to role
        var permissionManager = _serviceProvider.GetRequiredService<IPermissionManager>();
        await permissionManager.SetForRoleAsync("Owner", "Admin.Read", true);
        await permissionManager.SetForRoleAsync("Owner", "Admin.Write", true);
        
        await permissionManager.SetForRoleAsync("Developer", "Developer.Create", true);
        await permissionManager.SetForRoleAsync("Developer", "Developer.Delete", true);
        await permissionManager.SetForRoleAsync("Developer", "Developer.View", true);

        await permissionManager.SetForRoleAsync("Master", "Master.Create", true);
        await permissionManager.SetForRoleAsync("Master", "Master.Delete", true);
        await permissionManager.SetForRoleAsync("Master", "Master.View", true);
        await permissionManager.SetForRoleAsync("Master", "Admin.Read", true);
        
        await permissionManager.SetForRoleAsync("Master", "Developer.Create", false);
        await permissionManager.SetForRoleAsync("Master", "Developer.Delete", false);
        await permissionManager.SetForRoleAsync("Master", "Developer.View", false);
        
        await permissionManager.SetForRoleAsync("Guest", "Developer.View", true);

        // set user's roles
        var userManager = _serviceProvider.GetRequiredService<IdentityUserManager>();
        var ownerUserId = "ownerUser0".ToGuid().ToString();
        var ownerUser = await userManager.FindByIdAsync(ownerUserId);
        if (ownerUser == null)
        {
            ownerUser = new IdentityUser(Guid.Parse(ownerUserId), "ownerUser0", email: Guid.NewGuid().ToString("N") + "@ABP.IO");
            await userManager.CreateAsync(ownerUser);
        }
        await userManager.SetRolesAsync(ownerUser, new[]{"owner"});
        
        var developerId = "DeveloperUser0".ToGuid().ToString();
        var developerUser = await userManager.FindByIdAsync(developerId);
        if (developerUser == null)
        {
            developerUser = new IdentityUser(Guid.Parse(developerId), "DeveloperUser0", email: Guid.NewGuid().ToString("N") + "@ABP.IO");
            await userManager.CreateAsync(developerUser);
        }
        
        await userManager.SetRolesAsync(developerUser, new[]{"Developer"});
        
        var masterUserId = "MasterUser0".ToGuid().ToString();
        var masterUser = await userManager.FindByIdAsync(masterUserId);
        if (masterUser == null)
        {
            masterUser = new IdentityUser(Guid.Parse(masterUserId), "MasterUser0", email: Guid.NewGuid().ToString("N") + "@ABP.IO");
            await userManager.CreateAsync(masterUser);
        }
        await userManager.SetRolesAsync(masterUser, new[]{"Master", "Developer"});
        
        var guestUserId = "GuestUser0".ToGuid().ToString();
        var guestUser = await userManager.FindByIdAsync(guestUserId);
        if (guestUser == null)
        {
            guestUser = new IdentityUser(Guid.Parse(guestUserId), "GuestUser0", email: Guid.NewGuid().ToString("N") + "@ABP.IO");
            await userManager.CreateAsync(guestUser);
        }
        await userManager.SetRolesAsync(guestUser, new[]{"Guest"});
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _application.Shutdown();
        return Task.CompletedTask;
    }
}