﻿using Aevatar.Core.Abstractions;
using Aevatar.Core.Abstractions.Extensions;
using Aevatar.Extensions;
using Aevatar.GAgents.AIGAgent.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TokenUsageProjection.GAgents;

var builder = Host.CreateDefaultBuilder(args)
    .UseOrleansClient(client =>
    {
        client.UseLocalhostClustering()
            .UseMongoDBClient("mongodb://localhost:27017/?maxPoolSize=555")
            .AddMemoryStreams(AevatarCoreConstants.StreamProvider)
            .UseAevatar();
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .UseConsoleLifetime();

using var host = builder.Build();
await host.StartAsync();

var gAgentFactory = host.Services.GetRequiredService<IGAgentFactory>();
var gAgentManager = host.Services.GetRequiredService<IGAgentManager>();

Console.WriteLine("Select an option:");
Console.WriteLine("1. Demo token usage.");

var choice = Console.ReadLine();

switch (choice)
{
    case "1":
        await DemoAsync(gAgentFactory);
        break;
    default:
        Console.WriteLine("Invalid choice.");
        break;
}

async Task ListAllAvailableGAgentsAsync(IGAgentManager manager)
{
    var gAgents = manager.GetAvailableGAgentGrainTypes();
    foreach (var gAgent in gAgents)
    {
        Console.WriteLine(gAgent.ToString());
    }
}

async Task DemoAsync(IGAgentFactory factory)
{
    var aiGAgent = await factory.GetGAgentAsync<ISampleAIGAgent>("test".ToGuid());
    var projectionGAgent = await factory.GetGAgentAsync<IStateGAgent<TokenUsageProjectionGAgentState>>("test".ToGuid());
    await aiGAgent.PretendingChatAsync("whatever");
    await Task.Delay(3000);
    var state = await projectionGAgent.GetStateAsync();
    Console.WriteLine($"Token used: {state.TotalUsedToken}");
}