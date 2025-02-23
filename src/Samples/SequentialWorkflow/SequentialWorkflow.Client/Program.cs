// See https://aka.ms/new-console-template for more information

using Aevatar.Core.Abstractions;
using Aevatar.GAgents.Basic.BasicGAgents.GroupGAgent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SequentialWorkflow.GAgents.Common;
using SequentialWorkflow.GAgents.ProcessOrder;
using SequentialWorkflow.GAgents.Shipping;
using SequentialWorkflow.GAgents.ValidateUser;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .UseOrleansClient(client =>
    {
        client.UseLocalhostClustering()
            .AddMemoryStreams("InMemoryStreamProvider");
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .UseConsoleLifetime();

using IHost host = builder.Build();
await host.StartAsync();

IClusterClient client = host.Services.GetRequiredService<IClusterClient>();

// create agent
var groupGAgent = client.GetGrain<IGroupGAgent>(Guid.NewGuid());
var validateGAgent = client.GetGrain<IValidateUserGAgent>(Guid.NewGuid());
var processOrderGAgent = client.GetGrain<IProcessOrderGAgent>(Guid.NewGuid());
var shippingGAgent = client.GetGrain<IShippingGAgent>(Guid.NewGuid());

// organizational relationship
await groupGAgent.RegisterAsync(validateGAgent);
await validateGAgent.RegisterAsync(processOrderGAgent);
await processOrderGAgent.RegisterAsync(shippingGAgent);

// start the jobs
await groupGAgent.PublishEventAsync(new ValidateUserEvent(){ UserInfo = new UserInfo(){UserName = "Alice" ,VipLv = 0, Amount = 100, ShippingAddress = "New York"}});

await groupGAgent.PublishEventAsync(new ValidateUserEvent(){ UserInfo = new UserInfo(){UserName = "Bob" ,VipLv = 1, Amount = 5, ShippingAddress = "Washington"}});

await groupGAgent.PublishEventAsync(new ValidateUserEvent(){ UserInfo = new UserInfo(){UserName = "Fred" ,VipLv = 5, Amount = 2000, ShippingAddress = "Los Angeles"}});

