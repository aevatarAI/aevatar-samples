using Aevatar.GAgents.AIGAgent.Dtos;
using Aevatar.GAgents.GraphRetrievalAgent.GAgent;
using Aevatar.GAgents.GraphRetrievalAgent.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;


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

var graphAgent = client.GetGrain<IGraphRetrievalAgent>(Guid.NewGuid());

await graphAgent.InitializeAsync(new InitializeDto
{
    Instructions = "talk about movies",
    LLMConfig = new LLMConfigDto() { SystemLLM = "OpenAI" }
});


var schema = """
             Node properties:
             Person {name: STRING, born: INTEGER}
             Movie {tagline: STRING, title: STRING, released: INTEGER}

             Relationship properties:
             ACTED_IN {roles: LIST}
             REVIEWED {summary: STRING, rating: INTEGER}

             The relationships:
             (:Person)-[:ACTED_IN]->(:Movie)
             (:Person)-[:DIRECTED]->(:Movie)
             (:Person)-[:PRODUCED]->(:Movie)
             (:Person)-[:WROTE]->(:Movie)
             (:Person)-[:FOLLOWS]->(:Person)
             (:Person)-[:REVIEWED]->(:Movie)
             """;

var example =
    "USER INPUT: 'Which actors starred in the Matrix?' QUERY: MATCH (p:Person)-[:ACTED_IN]->(m:Movie) WHERE m.title = 'The Matrix' RETURN p.name";

await graphAgent.ConfigAsync(new GraphRetrievalConfig()
{
    Schema = schema,
    Example = example
});

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Assistant > Press enter with no prompt to exit.");

var appShutdownCancellationTokenSource = new CancellationTokenSource();
var cancellationToken = appShutdownCancellationTokenSource.Token;

while (!cancellationToken.IsCancellationRequested)
{
    // Prompt the user for a question.
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Assistant > What would you like to know?");

    // Read the user question.
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("User > ");
    var question = Console.ReadLine();

    // Exit the application if the user didn't type anything.
    if (string.IsNullOrWhiteSpace(question))
    {
        appShutdownCancellationTokenSource.Cancel();
        break;
    }

    var response = await graphAgent.InvokeLLMWithGraphRetrievalAsync(question);
    
    // Stream the LLM response to the console with error handling.
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write($"\nAssistant > {response}");
}

await host.StopAsync();


