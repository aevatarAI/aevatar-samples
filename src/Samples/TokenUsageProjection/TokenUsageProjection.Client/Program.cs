using Aevatar.Core.Abstractions.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TokenUsageProjection.GAgents;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.secrets.json", optional: true)
    .Build();

var signalRConfig = configuration.GetSection("SignalR");
var hubUrl = signalRConfig["HubUrl"];

var connection = new HubConnectionBuilder()
    .WithUrl(hubUrl!)
    .WithAutomaticReconnect() 
    .Build();

connection.On<string>("ReceiveResponse", (message) =>
{
    Console.WriteLine($"[Event] {message}");
});

await connection.StartAsync();

Console.WriteLine("Select an option:");
Console.WriteLine("1. Chat");
Console.WriteLine("2. Take snapshot");

var choice = Console.ReadLine();

while (true)
{
    switch (choice)
    {
        case "1":
            await PublishEventAsync("SubscribeAsync", typeof(ChatEvent), JsonConvert.SerializeObject(new ChatEvent
            {
                Message = "Test message"
            }));
            break;
        default:
            await PublishEventAsync("PublishEventAsync", typeof(TakeSnapshotEvent),
                JsonConvert.SerializeObject(new TakeSnapshotEvent()));
            break;
    }
    
    choice = Console.ReadLine();
}

async Task PublishEventAsync(string methodName, Type eventType, string eventJson)
{
    try
    {
        await SendEventWithRetry(connection, methodName,
            "TokenUsageProjection.GAgents.SampleAIGAgent",
            "test".ToGuid().ToString("N"),
            eventType.FullName!,
            eventJson);

        Console.WriteLine("✅ Success");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Abnormal: {ex.Message}");
    }
}

async Task SendEventWithRetry(HubConnection conn, string methodName, string grainType, string grainKey, string eventTypeName, string eventJson)
{
    var grainId = GrainId.Create(grainType, grainKey);
    const int maxRetries = 3;
    var retryCount = 0;

    while (retryCount < maxRetries)
    {
        try
        {
            if (conn.State != HubConnectionState.Connected)
            {
                Console.WriteLine("Connection broke, retrying...");
                await conn.StartAsync();
            }

            var signalRGAgentGrainId = await connection.InvokeAsync<GrainId>(methodName, grainId, eventTypeName, eventJson);
            Console.WriteLine($"SignalRGAgent GrainId: {signalRGAgentGrainId.ToString()}");
            return;
        }
        catch (Exception ex)
        {
            retryCount++;
            Console.WriteLine($"❌ Failed（Retry {retryCount}/{maxRetries}）: {ex.Message}");
            if (retryCount >= maxRetries)
            {
                throw;
            }
            await Task.Delay(1000 * retryCount);
        }
    }
}