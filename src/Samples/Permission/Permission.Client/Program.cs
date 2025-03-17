// See https://aka.ms/new-console-template for more information

using Aevatar.Core.Abstractions.Extensions;
using Aevatar.PermissionManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Permission.GAgents;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .UseOrleansClient(client =>
    {
        client.UseLocalhostClustering()
            .UseMongoDBClient("mongodb://localhost:27017/?maxPoolSize=555")
            .AddMemoryStreams("InMemoryStreamProvider");

    })
    .ConfigureLogging(logging => logging.AddConsole())
    .UseConsoleLifetime();

using IHost host = builder.Build();
await host.StartAsync();

IClusterClient client = host.Services.GetRequiredService<IClusterClient>();

var allPermissionInfos = GAgentPermissionHelper.GetAllPermissionInfos();
foreach(var permissionInfo in allPermissionInfos){
    Console.WriteLine(JsonConvert.SerializeObject(permissionInfo));
}

// developer agent
var developerGAgent = client.GetGrain<IDeveloperGAgent>(Guid.NewGuid());

// check developer permissions
RequestContext.Set("CurrentUser", new UserContext
{
    UserId = "DeveloperUser0".ToGuid(),
    Roles = new []{"Developer"}
});
var developerDesc = await developerGAgent.GetDescriptionAsync();
Console.WriteLine(developerDesc);
await developerGAgent.GetAsync();
await developerGAgent.CreateAsync();
await developerGAgent.DelAsync();
Console.WriteLine("developer has permission: Developer.View、Developer.Create、Developer.Delete");

var masterGAgent = client.GetGrain<IMasterGAgent>(Guid.NewGuid());
try
{
    await masterGAgent.GetAsync();
}
catch (Exception e)
{
    Console.WriteLine($"developer has not permission Master.View: {e.Message}");
}

// check master permissions
RequestContext.Set("CurrentUser", new UserContext
{
    UserId = "MasterUser0".ToGuid(),
    Roles = new []{"Master", "Developer"}
});
await masterGAgent.GetAsync();
await masterGAgent.CreateAsync();
await masterGAgent.DelAsync();

await developerGAgent.GetAsync();
await developerGAgent.CreateAsync();
await developerGAgent.DelAsync();
Console.WriteLine("master has permission: Developer.View、Developer.Create、Developer.Delete、Master.View、Master.Create、Master.Delete");

// check Guest permissions
RequestContext.Set("CurrentUser", new UserContext
{
    UserId = "GuestUser0".ToGuid(),
    Roles = new []{"Guest"}
});
await developerGAgent.GetAsync();
Console.WriteLine("Guest has permission: Developer.View");
try
{
    await masterGAgent.GetAsync();
}
catch (Exception e)
{
    Console.WriteLine($"guest has not permission Master.View: {e.Message}");
}

// check Admin&Class permissions
var adminGAgent = client.GetGrain<IAdminGAgent>(Guid.NewGuid());
RequestContext.Set("CurrentUser", new UserContext
{
    UserId = "OwnerUser0".ToGuid(),
    Roles = new []{"Owner"}
});
await adminGAgent.GetAsync();
await adminGAgent.CreateAsync();
Console.WriteLine("Owner has permission: Admin.Read、Admin.Write");


RequestContext.Set("CurrentUser", new UserContext
{
    UserId = "MasterUser0".ToGuid(),
    Roles = new []{"Master"}
});
await adminGAgent.GetAsync();
Console.WriteLine("Master has permission: Admin.Read");

try
{
    await adminGAgent.CreateAsync();
}
catch (Exception e)
{
    Console.WriteLine($"master has not permission Admin.Write: {e.Message}");
}

