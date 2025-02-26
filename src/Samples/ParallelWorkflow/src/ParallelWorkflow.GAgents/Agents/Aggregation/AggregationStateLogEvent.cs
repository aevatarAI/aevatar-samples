using Aevatar.Core.Abstractions;

namespace ParallelWorkflow.GAgents.Agents.Aggregation;

[GenerateSerializer]
public class AggregationStateLogEvent: StateLogEventBase<AggregationStateLogEvent>
{
    
}

[GenerateSerializer]
public class TransferAggregateStateLogEvent: AggregationStateLogEvent
{
    [Id(0)]
    public Dictionary<string, List<TransferInfo>> Transfers { get; set; } = new();
}