using Aevatar.Core;
using Aevatar.Core.Abstractions;
using Microsoft.Extensions.Logging;
using SequentialWorkflow.GAgents.ProcessOrder;
using Serilog;

namespace SequentialWorkflow.GAgents.Shipping;

public class ShippingGAgent : GAgentBase<ShippingState, ShippingLogEvent>, IShippingGAgent
{
    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("Ship to the user");
    }

    [EventHandler]
    public async Task HandleEventAsync(ProcessOrderEvent @event)
    {
        Log.Logger.Information($"{nameof(ShippingGAgent)} receive {nameof(ProcessOrderEvent)} Event");

        if (@event.UserInfo.ShippingAddress.IsNullOrEmpty())
        {
            Log.Logger.Error(
                $"{nameof(ShippingGAgent)}-{nameof(ProcessOrderEvent)} User:{@event.UserInfo.UserName} shipping address is empty");
            return;
        }

        Log.Logger.Information(
            $"{nameof(ShippingGAgent)} receive {nameof(ProcessOrderEvent)} Event and it's has been done");
    }
}

public interface IShippingGAgent : IGAgent
{
}