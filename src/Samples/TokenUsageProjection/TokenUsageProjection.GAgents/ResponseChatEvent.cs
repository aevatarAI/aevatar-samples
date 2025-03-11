using Aevatar.SignalR;

namespace TokenUsageProjection.GAgents;

[GenerateSerializer]
public class ResponseChatEvent : ResponseToPublisherEventBase
{
    [Id(0)] public long TotalUsedToken { get; set; }
    [Id(1)] public long UsedInputToken { get; set; }
    [Id(2)] public long UsedOutputToken { get; set; }
}