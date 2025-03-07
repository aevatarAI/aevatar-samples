using Aevatar.Core.Abstractions;
using Aevatar.GAgents.AIGAgent.Agent;
using Aevatar.GAgents.AIGAgent.State;
using Microsoft.Extensions.Logging;

namespace TokenUsageProjection.GAgents;

[GenerateSerializer]
public class SampleAIGAgentState : AIGAgentStateBase
{
    [Id(0)] public int LatestTotalUsageToken { get; set; }
    [Id(1)] public int LatestInputToken { get; set; }
    [Id(2)] public int LatestOutputToken { get; set; }
    [Id(3)] public DateTime LatestUpdateTime { get; set; }
    [Id(4)] public DateTime CreateTime { get; set; }
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
        var tokenUsage = new TokenUsageStateLogEvent
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
            State.LatestInputToken = tokenUsageStateLogEvent.InputToken;
            State.LatestOutputToken = tokenUsageStateLogEvent.OutputToken;
            State.LatestUpdateTime = DateTime.UtcNow;
        }
        else
        {
            State.LatestTotalUsageToken = 0;
            State.LatestInputToken = 0;
            State.LatestOutputToken = 0;
            switch (@event)
            {
                case InitializeStateLogEvent initializeStateLogEvent:
                    State.CreateTime = initializeStateLogEvent.CreateTime;
                    break;
            }
        }
    }

    [GenerateSerializer]
    public class InitializeStateLogEvent : StateLogEventBase<SampleAIStateLogEvent>
    {
        [Id(0)] public DateTime CreateTime { get; set; }
    }
}