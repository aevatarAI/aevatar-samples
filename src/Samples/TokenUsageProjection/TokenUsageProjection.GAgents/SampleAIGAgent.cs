using Aevatar.Core.Abstractions;
using Aevatar.GAgents.AIGAgent.Agent;
using Aevatar.GAgents.AIGAgent.State;
using Microsoft.Extensions.Logging;

namespace TokenUsageProjection.GAgents;

[GenerateSerializer]
public class SampleAIGAgentState : AIGAgentStateBase
{
    [Id(0)] public int LatestTotalUsageToken { get; set; }
}

[GenerateSerializer]
public class SampleAIStateLogEvent : StateLogEventBase<SampleAIStateLogEvent>;

public interface ISampleAIGAgent : IAIGAgent, IStateGAgent<SampleAIGAgentState>
{
    Task PretendingChatAsync(string message);
}

[GAgent]
public class SampleAIGAgent : AIGAgentBase<SampleAIGAgentState, SampleAIStateLogEvent>, ISampleAIGAgent
{
    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An AI GAgent sample to test state projection.");
    }

    public async Task PretendingChatAsync(string message)
    {
        Logger.LogInformation("Call PretendingChatAsync");
        var tokenUsage = new TokenUsageStateLogEvent()
        {
            GrainId = this.GetPrimaryKey(),
            InputToken = message.Length,
            OutputToken = 2000,
            TotalUsageToken = 2000 + message.Length
        };
        RaiseEvent(tokenUsage);
        await ConfirmEvents();
    }

    protected override void AIGAgentTransitionState(SampleAIGAgentState state, StateLogEventBase<SampleAIStateLogEvent> @event)
    {
        if (@event is TokenUsageStateLogEvent tokenUsageStateLogEvent)
        {
            State.LatestTotalUsageToken = tokenUsageStateLogEvent.TotalUsageToken;
        }
        else
        {
            State.LatestTotalUsageToken = 0;
        }
    }
}