using Aevatar.Core;
using Aevatar.Core.Abstractions;
using Microsoft.Extensions.Logging;
using Serilog;

namespace SequentialWorkflow.GAgents.ValidateUser;

public class ValidateUserGAgent : GAgentBase<ValidateUserState, ValidateUserLogEvent>, IValidateUserGAgent
{
    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("Validate User Agent: Verify if the user has permission to purchase the product.");
    }

    [EventHandler]
    public async Task HandleEventAsync(ValidateUserEvent @event)
    {
        Log.Logger.Information($"{nameof(ValidateUserGAgent)} receive {nameof(ValidateUserEvent)} Event");
        if (@event.UserInfo.VipLv < 1)
        {
            Log.Logger.Error(
                $"{nameof(ValidateUserGAgent)}-{nameof(ValidateUserEvent)} User:{@event.UserInfo.UserName} VipLv is low");
            return;
        }

        Log.Logger.Information(
            $"{nameof(ValidateUserGAgent)} receive {nameof(ValidateUserEvent)} Event and it's has been done");
        await PublishAsync(new ValidateUserPassEvent() { UserInfo = @event.UserInfo });
    }
}

public interface IValidateUserGAgent : IGAgent
{
}