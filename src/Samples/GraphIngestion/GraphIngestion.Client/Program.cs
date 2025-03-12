using GraphIngestion.GAgents.Model;
using GraphIngestion.GAgents.SportsAgent;
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

var sportsGAgent = client.GetGrain<ISportsGAgent>(Guid.NewGuid());

var club1 = new Node
{
    Labels = new[] { "Club" },
    Properties = new Dictionary<string, object>
    {
        { "Name", "Real Madrid" },
        { "City", "Madrid" }
    },
    MatchKey = "Name"
};

var club2 = new Node
{
    Labels = new[] { "Club" },
    Properties = new Dictionary<string, object>
    {
        { "Name", "Barcelona" },
        { "City", "Barcelona" }
    },
    MatchKey = "Name"
};

var player = new Node
{
    Labels = new[] { "Player" },
    Properties = new Dictionary<string, object>
    {
        { "Name", "Luka Modric" },
        { "Country", "Croatia" }
    },
    MatchKey = "Name"
};

var relationships = new[]
{
    new Relationship
    {
        Type = "PLAYS_FOR",
        StartNode = player,
        EndNode = club1
    },
    new Relationship
    {
        Type = "RIVAL",
        StartNode = club1,
        EndNode = club2
    }
};

await sportsGAgent.CreateDataAsync(new[] { club1, club2, player }, relationships); 
Console.WriteLine("Data created successfully!");

var result1 = await sportsGAgent.GetClubPlayers("Real Madrid");
Console.WriteLine($"Players in Real Madrid: {string.Join(", ", result1)}");

var result2 = await sportsGAgent.GetClubRivals("Real Madrid");
Console.WriteLine($"Rivals of Real Madrid: {string.Join(", ", result2)}");



