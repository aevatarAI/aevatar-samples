using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Aevatar.Core.Abstractions;
using Aevatar.GAgents.AI.Options;
using Aevatar.GAgents.SemanticKernel.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;

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
        services.Configure<AzureOpenAIEmbeddingsConfig>(context.Configuration.GetSection("AIServices:AzureOpenAIEmbeddings"));
        services.Configure<GeminiConfig>(context.Configuration.GetSection("AIServices:Gemini"));
        services.Configure<QdrantConfig>(context.Configuration.GetSection("VectorStores:Qdrant"));
        services.Configure<RagConfig>(context.Configuration.GetSection("Rag"));

        services.AddSemanticKernel()
            .AddAzureOpenAI()
            .AddQdrantVectorStore()
            .AddAzureOpenAITextEmbedding()
            .AddAzureAIInference()
            .AddGemini();
    })
    .UseConsoleLifetime();

using var host = builder.Build();

await host.RunAsync();