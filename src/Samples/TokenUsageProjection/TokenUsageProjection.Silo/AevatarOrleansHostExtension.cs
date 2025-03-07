using Aevatar.Core;
using Aevatar.Core.Abstractions;
using Aevatar.EventSourcing.MongoDB.Hosting;
using Aevatar.Extensions;
using Aevatar.GAgents.AI.Options;
using Aevatar.GAgents.SemanticKernel.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Orleans.Hosting;

namespace AevatarTemplate.Silo;

public static class AevatarOrleansHostExtension
{
    public static IHostBuilder UseOrleansConfiguration(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseOrleans((context, siloBuilder) =>
            {
                const string mongoDbDefaultConnectString = "mongodb://localhost:27017/?maxPoolSize=555";
                siloBuilder
                    .UseLocalhostClustering()
                    .UseMongoDBClient(mongoDbDefaultConnectString)
                    .AddMongoDBGrainStorage("PubSubStore", options =>
                    {
                        options.CollectionPrefix = "StreamStorage";
                        options.DatabaseName = "AevatarDb";
                    })
                    .ConfigureLogging(logging => { logging.SetMinimumLevel(LogLevel.Debug).AddConsole(); })
                    .AddMongoDbStorageBasedLogConsistencyProvider("LogStorage", options =>
                    {
                        options.ClientSettings =
                            MongoClientSettings.FromConnectionString(mongoDbDefaultConnectString);
                        options.Database = "AevatarDb";
                    })
                    .AddMemoryStreams("Aevatar")
                    .ConfigureServices(services =>
                    {
                        services.AddSemanticKernel()
                            .AddAzureOpenAI()
                            .AddQdrantVectorStore()
                            .AddAzureOpenAITextEmbedding();
                        services.AddTransient<IStateDispatcher, StateDispatcher>();
                        services.AddTransient<IStateProjector, TestStateProjector>();
                    })
                    .UseAevatar<AevatarSiloModule>();
            })
            .UseConsoleLifetime();
    }
}