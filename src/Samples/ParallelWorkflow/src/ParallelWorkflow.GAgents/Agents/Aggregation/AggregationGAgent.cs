using AElf.Client.Dto;
using Aevatar.Core;
using Aevatar.Core.Abstractions;
using Microsoft.Extensions.Logging;
using ParallelWorkflow.GAgents.Agents.Events;

namespace ParallelWorkflow.GAgents.Agents.Aggregation;

public interface IAggregationGAgent : IGAgent
{
    Task<Dictionary<string, List<TransferInfo>>> GetResultAsync();
}


public class AggregationGAgent : GAgentBase<AggregationState, AggregationStateLogEvent>, IAggregationGAgent
{
    public override async Task<string> GetDescriptionAsync()
    {
        return "Aggregation GAgent";
    }

    [EventHandler]
    public async Task HandleEventAsync(AggregationEvent @event)
    {
        Logger.LogInformation("Handle aggregation event.");
        
        var transfers = FilterTransfer(@event.Transactions);
        var result = new Dictionary<string, List<TransferInfo>>();
        foreach (var transfer in transfers)
        {
            if (!result.TryGetValue(transfer.From, out var value))
            {
                value = new List<TransferInfo>();
            }

            value.Add(transfer);
            result[transfer.From] = value;
        }

        RaiseEvent(new TransferAggregateStateLogEvent
        {
            Transfers = result
        });
        await ConfirmEvents();
    }

    public async Task<Dictionary<string, List<TransferInfo>>> GetResultAsync()
    {
        return State.Transfers;
    }

    protected override void GAgentTransitionState(AggregationState state,
        StateLogEventBase<AggregationStateLogEvent> @event)
    {
        switch (@event)
        {
            case TransferAggregateStateLogEvent transferAggregateStateLogEvent:
                State.Transfers = transferAggregateStateLogEvent.Transfers;
                break;
        }
    }

    private List<TransferInfo> FilterTransfer(List<TransactionResultDto> transactionResults)
    {
        // Get transfer information based on the transaction result events
        return
        [
            new TransferInfo { From = Guid.NewGuid().ToString("N"), To = Guid.NewGuid().ToString("N"), Amount = 10 },
            new TransferInfo { From = Guid.NewGuid().ToString("N"), To = Guid.NewGuid().ToString("N"), Amount = 20 }
        ];
    }
}