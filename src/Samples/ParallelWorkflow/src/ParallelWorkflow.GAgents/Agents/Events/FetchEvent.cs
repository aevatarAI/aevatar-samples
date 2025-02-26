using Aevatar.Core.Abstractions;

namespace ParallelWorkflow.GAgents.Agents.Events;

[GenerateSerializer]
public class FetchEvent : EventBase
{
    public long StartBlockHeight { get; set; }
    public long EndBlockHeight { get; set; }
}