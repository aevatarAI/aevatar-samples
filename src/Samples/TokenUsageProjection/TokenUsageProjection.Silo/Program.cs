using Aevatar.Core;
using Aevatar.Core.Abstractions;
using Aevatar.Extensions;
using Aevatar.GAgents.AI.Options;
using Aevatar.GAgents.SemanticKernel.Extensions;
using Aevatar.PermissionManagement.Extensions;
using Aevatar.SignalR;
using AevatarTemplate.Silo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.secrets.json", optional: true)
    .Build();

builder.Host.UseOrleans(silo =>
    {
        silo.AddMemoryGrainStorage("PubSubStore")
            .AddLogStorageBasedLogConsistencyProvider("LogStorage")
            .UseLocalhostClustering()
            .ConfigureServices(services =>
            {
                services.AddTransient<IGAgentFactory, GAgentFactory>();
            })
            .ConfigureLogging(logging => { logging.SetMinimumLevel(LogLevel.Information).AddConsole(); })
            .UseAevatarPermissionManagement()
            .UseAevatar()
            .UseSignalR()
            .RegisterHub<AevatarSignalRHub>();
        silo.Services.AddSingleton<IStateDispatcher, StateDispatcher>();
        silo.Services.AddSingleton<IStateProjector, TestStateProjector>();
        silo.AddLogStorageBasedLogConsistencyProvider("LogStorage");
        silo.AddMemoryStreams("Aevatar");
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<AzureOpenAIConfig>(context.Configuration.GetSection("AIServices:AzureOpenAI"));
        services.Configure<QdrantConfig>(context.Configuration.GetSection("VectorStores:Qdrant"));
        services.Configure<AzureOpenAIEmbeddingsConfig>(
            context.Configuration.GetSection("AIServices:AzureOpenAIEmbeddings"));
        services.Configure<RagConfig>(context.Configuration.GetSection("Rag"));

        services.AddSemanticKernel()
            .AddQdrantVectorStore()
            .AddAzureOpenAITextEmbedding();
    });

builder.WebHost.UseKestrel((_, kestrelOptions) =>
{
    kestrelOptions.ListenLocalhost( 5001);
});

builder.Services.AddSignalR().AddOrleans();

builder.Services.AddHostedService<AevatarSiloHostedService>();

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();
app.MapHub<AevatarSignalRHub>("/aevatarHub");
await app.RunAsync();