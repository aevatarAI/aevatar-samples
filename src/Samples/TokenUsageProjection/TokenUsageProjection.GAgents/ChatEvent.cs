using Aevatar.Core.Abstractions;

namespace TokenUsageProjection.GAgents;

[GenerateSerializer]
public class ChatEvent : EventBase
{
    [Id(0)] public string Message { get; set; }
}