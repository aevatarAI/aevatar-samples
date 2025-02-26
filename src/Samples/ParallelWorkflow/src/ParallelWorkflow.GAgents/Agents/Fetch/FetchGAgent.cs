using AElf.Client.Dto;
using Aevatar.Core;
using Aevatar.Core.Abstractions;
using Microsoft.Extensions.Logging;
using ParallelWorkflow.GAgents.Agents.Events;

namespace ParallelWorkflow.GAgents.Agents.Fetch;

public interface IFetchGAgent : IGAgent
{
}

public class FetchGAgent : GAgentBase<FetchState, FetchStateLogEvent>, IFetchGAgent
{
    public override async Task<string> GetDescriptionAsync()
    {
        return "Fetch GAgent";
    }
    
    [EventHandler]
    public async Task HandleEventAsync(FetchEvent @event)
    {
        Logger.LogInformation("Handle fetch event.");
        
        if (@event.StartBlockHeight > @event.EndBlockHeight)
        {
            throw new ArgumentNullException(nameof(@event));
        }
        var transactions = await QueryTransactionAsync(@event.StartBlockHeight, @event.EndBlockHeight);
        await PublishAsync(new AggregationEvent { Transactions = transactions });
    }

    private async Task<List<TransactionResultDto>> QueryTransactionAsync(long startBlockHeight, long endBlockHeight)
    {
        // Query transaction results by height
        await Task.Delay(1000);
        return new List<TransactionResultDto>();
    }
}