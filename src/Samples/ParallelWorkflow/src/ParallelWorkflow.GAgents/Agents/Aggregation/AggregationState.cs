using Aevatar.Core.Abstractions;

namespace ParallelWorkflow.GAgents.Agents.Aggregation;

[GenerateSerializer]
public class AggregationState : StateBase
{
    [Id(0)]
    public Dictionary<string, List<TransferInfo>> Transfers { get; set; } = new();
}