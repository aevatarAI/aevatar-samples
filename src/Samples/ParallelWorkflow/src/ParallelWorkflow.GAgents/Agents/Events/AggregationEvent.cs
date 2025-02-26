using AElf.Client.Dto;
using Aevatar.Core.Abstractions;

namespace ParallelWorkflow.GAgents.Agents.Events;

[GenerateSerializer]
public class AggregationEvent : EventBase
{
    public List<TransactionResultDto> Transactions { get; set; } = new();
}