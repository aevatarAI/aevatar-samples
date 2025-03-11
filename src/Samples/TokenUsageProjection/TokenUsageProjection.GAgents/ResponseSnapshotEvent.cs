using Aevatar.SignalR;

namespace TokenUsageProjection.GAgents;

[GenerateSerializer]
public class ResponseSnapshotEvent : ResponseToPublisherEventBase
{
    [Id(0)] public DateTime From { get; set; }
    [Id(1)] public DateTime To { get; set; }
    [Id(2)] public long UsedToken { get; set; }
    [Id(3)] public long InputToken { get; set; }
    [Id(4)] public long OutputToken { get; set; }
}