using Aevatar.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

var builder = Host.CreateDefaultBuilder(args)
    .UseOrleans(silo =>
    {
        silo.AddMemoryGrainStorage("Default")
            .AddMemoryStreams(AevatarCoreConstants.StreamProvider)
            .AddMemoryGrainStorage("PubSubStore")
            .AddLogStorageBasedLogConsistencyProvider("LogStorage")
            .UseLocalhostClustering()
            .ConfigureLogging(logging => logging.AddConsole());
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IDriver>(_ => GraphDatabase.Driver(
            context.Configuration["Neo4j:Uri"],
            AuthTokens.Basic(
                context.Configuration["Neo4j:User"],
                context.Configuration["Neo4j:Password"]
            )
        ));
    })
    .UseConsoleLifetime();

using var host = builder.Build();

await host.RunAsync();