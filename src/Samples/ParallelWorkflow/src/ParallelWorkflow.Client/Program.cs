using Aevatar.Core.Abstractions;
using Aevatar.GAgents.Basic.BasicGAgents.GroupGAgent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using ParallelWorkflow.GAgents.Agents;
using ParallelWorkflow.GAgents.Agents.Aggregation;
using ParallelWorkflow.GAgents.Agents.Events;
using ParallelWorkflow.GAgents.Agents.Fetch;

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

var jobs = new List<Tuple<Guid,long,long>>();
const int step = 10;
for (var i = 0; i < 10; i++)
{
    var startHeight = 1 + i * step;
    jobs.Add(new Tuple<Guid, long, long>(Guid.NewGuid(), startHeight, startHeight + step - 1));
}
var tasks = jobs.Select(async o =>
{
    var groupGAgent = client.GetGrain<IGroupGAgent>(o.Item1);
    var fetchGAgent = client.GetGrain<IFetchGAgent>(o.Item1);
    var aggregationGAgent = client.GetGrain<IAggregationGAgent>(o.Item1);
    await groupGAgent.RegisterAsync(fetchGAgent);
    await fetchGAgent.RegisterAsync(aggregationGAgent);
    await groupGAgent.PublishEventAsync(new FetchEvent { StartBlockHeight = o.Item2, EndBlockHeight = o.Item3 });
});
await tasks.WhenAll();

await Task.Delay(2000);

var queryResultTasks = jobs.Select(async o =>
{
    var aggregationGAgent = client.GetGrain<IAggregationGAgent>(o.Item1);
    return await aggregationGAgent.GetResultAsync();
});
var result = await queryResultTasks.WhenAll();

foreach (var item in result)
{
    Console.WriteLine($"Address count: {item.Keys.Count}");
}