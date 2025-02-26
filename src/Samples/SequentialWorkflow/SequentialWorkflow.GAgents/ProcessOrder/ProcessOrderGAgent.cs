using Aevatar.Core;
using Aevatar.Core.Abstractions;
using Microsoft.Extensions.Logging;
using SequentialWorkflow.GAgents.ValidateUser;
using Serilog;

namespace SequentialWorkflow.GAgents.ProcessOrder;

public class ProcessOrderGAgent : GAgentBase<ProcessOrderState, ProcessOrderLogEvent>, IProcessOrderGAgent
{
    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("Process user order.");
    }

    [EventHandler]
    public async Task HandleEventAsync(ValidateUserPassEvent @event)
    {
        Log.Logger.Information($"{nameof(ProcessOrderGAgent)} receive {nameof(ValidateUserPassEvent)} Event");

        if (@event.UserInfo.Amount < 100)
        {
            Log.Logger.Error(
                $"{nameof(ProcessOrderGAgent)}-{nameof(ValidateUserPassEvent)} User:{@event.UserInfo.UserName} money is not enough");
            return;
        }

        Log.Logger.Information(
            $"{nameof(ProcessOrderGAgent)} receive {nameof(ValidateUserPassEvent)} Event and it's has been done");
        await PublishAsync(new ProcessOrderEvent() { UserInfo = @event.UserInfo });
    }
}

public interface IProcessOrderGAgent : IGAgent
{
}