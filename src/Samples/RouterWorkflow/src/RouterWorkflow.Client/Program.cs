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
    LLMConfig = new LLMConfigDto() { SystemLLM = "OpenAI" }
});
        
        
var researcherGAgent = client.GetGrain<IResearcherGAgent>(Guid.NewGuid());
await researcherGAgent.InitializeAsync(new InitializeDto
{
    Instructions = "You are a researcher",
    LLMConfig = new LLMConfigDto() { SystemLLM = "OpenAI" }
});
var researcherGAgentEvents = await researcherGAgent.GetAllSubscribedEventsAsync();
await routerGAgent.AddAgentDescription(researcherGAgent.GetType(), researcherGAgentEvents);
        
var writerGAgent = client.GetGrain<IWriterGAgent>(Guid.NewGuid());
await writerGAgent.InitializeAsync(new InitializeDto
{
    Instructions = "You are a writer",
    LLMConfig = new LLMConfigDto() { SystemLLM = "OpenAI" }
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

var researchResult = string.Empty;
while (researchResult.IsNullOrWhiteSpace())
{
    researchResult = await researcherGAgent.GetResultAsync();
    if (researchResult.IsNullOrWhiteSpace())
    {
        await Task.Delay(5000);
        continue;
    }

    Console.WriteLine("Research result:");
    Console.WriteLine(researchResult);
    break;
}

Console.WriteLine();

var article = string.Empty;
while (article.IsNullOrWhiteSpace())
{
    article = await writerGAgent.GetArticleAsync();
    if (article.IsNullOrWhiteSpace())
    {
        await Task.Delay(5000);
        continue;
    }

    Console.WriteLine("Report:");
    Console.WriteLine(article);
    break;
}