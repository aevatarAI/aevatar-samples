using Aevatar.Core.Abstractions;
using Aevatar.GAgents.AI.Options;
using Aevatar.GAgents.Neo4jStore.Extensions;
using Aevatar.GAgents.Neo4jStore.Options;
using Aevatar.GAgents.SemanticKernel.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


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
        services.Configure<AzureOpenAIConfig>(context.Configuration.GetSection("AIServices:AzureOpenAI"));
        services.Configure<QdrantConfig>(context.Configuration.GetSection("VectorStores:Qdrant"));
        services.Configure<AzureOpenAIEmbeddingsConfig>(context.Configuration.GetSection("AIServices:AzureOpenAIEmbeddings"));
        services.Configure<RagConfig>(context.Configuration.GetSection("Rag"));
        services.Configure<Neo4jDriverConfig>(context.Configuration.GetSection("Neo4j"));
        
        services.AddSemanticKernel()
            .AddAzureOpenAI()
            .AddQdrantVectorStore()
            .AddAzureOpenAITextEmbedding()
            .AddNeo4JStore();
    })
    .UseConsoleLifetime();

using var host = builder.Build();

await host.RunAsync();