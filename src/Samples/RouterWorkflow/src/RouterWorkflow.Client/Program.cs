using Aevatar.Core.Abstractions;
using Aevatar.GAgents.AIGAgent.Dtos;
using Aevatar.GAgents.Basic.BasicGAgents.GroupGAgent;
using Aevatar.GAgents.Router.GAgents;
using Aevatar.GAgents.Router.GEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using RouterWorkflow.GAgents.Agents;
using RouterWorkflow.GAgents.Agents.Events;
using RouterWorkflow.GAgents.Agents.Researcher;
using RouterWorkflow.GAgents.Agents.Writer;

var builder = Host.CreateDefaultBuilder(args)
    .UseOrleansClient(client =>
    {
        client.UseLocalhostClustering()
            .AddMemoryStreams("InMemoryStreamProvider");
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .UseConsoleLifetime();

using var host = builder.Build();
await host.StartAsync();

var client = host.Services.GetRequiredService<IClusterClient>();

var routerGAgent = client.GetGrain<IRouterGAgent>(Guid.NewGuid());

await routerGAgent.InitializeAsync(new InitializeDto
{
    Instructions = "You are a router agent",
    LLM = "AzureOpenAI"
});
        
        
var researcherGAgent = client.GetGrain<IResearcherGAgent>(Guid.NewGuid());
await researcherGAgent.InitializeAsync(new InitializeDto
{
    Instructions = "You are a researcher",
    LLM = "AzureOpenAI"
});
var researcherGAgentEvents = await researcherGAgent.GetAllSubscribedEventsAsync();
await routerGAgent.AddAgentDescription(researcherGAgent.GetType(), researcherGAgentEvents);
        
var writerGAgent = client.GetGrain<IWriterGAgent>(Guid.NewGuid());
await writerGAgent.InitializeAsync(new InitializeDto
{
    Instructions = "You are a writer",
    LLM = "AzureOpenAI"
});
var writerGAgentEvents = await writerGAgent.GetAllSubscribedEventsAsync();
await routerGAgent.AddAgentDescription(writerGAgent.GetType(), writerGAgentEvents);
        
var groupGAgent = client.GetGrain<IGroupGAgent>(Guid.NewGuid());
await groupGAgent.RegisterAsync(routerGAgent);
await groupGAgent.RegisterAsync(researcherGAgent);
await groupGAgent.RegisterAsync(writerGAgent);
        
await groupGAgent.PublishEventAsync(new BeginTaskGEvent()
{
    TaskDescription = "Research AI agents and write a brief report about them."
});

await Task.Delay(100000);

var researchResult = await researcherGAgent.GetResultAsync();
Console.WriteLine("Research result:");
Console.WriteLine(researchResult);

Console.WriteLine();

var article = await writerGAgent.GetArticleAsync();
Console.WriteLine("Report:");
Console.WriteLine(article);