using System.Diagnostics;
using Aevatar.Core;
using Aevatar.Core.Abstractions;

namespace TokenUsageProjection.GAgents;

[GenerateSerializer]
public class TokenUsageProjectionGAgentState : StateBase
{
    [Id(0)] public long TotalUsedToken { get; set; }
    [Id(1)] public long TotalInputToken { get; set; }
    [Id(2)] public long TotalOutputToken { get; set; }
    [Id(3)] public long ElapsedSeconds { get; set; }
    [Id(4)] public HashSet<Guid> ActivatedGAgentPrimaryKeys { get; set; } = [];
    
    public decimal GetUsedInputTokenCount(TimeSpan? timeSpan = null)
    {
        timeSpan ??= new TimeSpan(0, 5, 0);
        return (decimal)TotalInputToken / ElapsedSeconds * timeSpan.Value.Seconds;
    }

    public decimal GetUsedOutputTokenCount(TimeSpan? timeSpan = null)
    {
        timeSpan ??= new TimeSpan(0, 5, 0);
        return (decimal)TotalOutputToken / ElapsedSeconds * timeSpan.Value.Seconds;
    }
}

[GenerateSerializer]
public class TokenUsageProjectionStateLogEvent : StateLogEventBase<TokenUsageProjectionStateLogEvent>;

[GAgent]
public class TokenUsageProjectionGAgent : StateProjectionGAgentBase<SampleAIGAgentState, TokenUsageProjectionGAgentState
    , TokenUsageProjectionStateLogEvent>
{
    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("This is a GAgent for testing token usage projection.");
    }

    protected override async Task HandleStateAsync(StateWrapper<SampleAIGAgentState> projectionStateWrapper)
    {
        var projectionState = projectionStateWrapper.State;
        RaiseEvent(new TokenUsageStateLogEvent
        {
            TotalUsageToken = projectionState.LatestTotalUsageToken,
            InputToken = projectionState.LatestInputToken,
            OutputToken = projectionState.LatestOutputToken,
            ElapsedSeconds = (projectionState.LatestUpdateTime - projectionState.CreateTime).Seconds,
        });
        RaiseEvent(new MaybeNewGAgentStateLogEvent
        {
            GAgentPrimaryKey = projectionStateWrapper.GrainId.GetGuidKey()
        });
        await ConfirmEvents();
    }

    protected override void GAgentTransitionState(TokenUsageProjectionGAgentState state,
        StateLogEventBase<TokenUsageProjectionStateLogEvent> @event)
    {
        switch (@event)
        {
            case TokenUsageStateLogEvent tokenUsageStateLogEvent:
                State.TotalUsedToken += tokenUsageStateLogEvent.TotalUsageToken;
                State.TotalInputToken += tokenUsageStateLogEvent.InputToken;
                State.TotalOutputToken += tokenUsageStateLogEvent.OutputToken;
                State.ElapsedSeconds = tokenUsageStateLogEvent.ElapsedSeconds;
                break;
            case MaybeNewGAgentStateLogEvent maybeNewGAgentStateLogEvent:
                State.ActivatedGAgentPrimaryKeys.AddIfNotContains(maybeNewGAgentStateLogEvent.GAgentPrimaryKey);
                break;
        }
    }

    [GenerateSerializer]
    public class TokenUsageStateLogEvent : StateLogEventBase<TokenUsageProjectionStateLogEvent>
    {
        [Id(0)] public long InputToken { get; set; }
        [Id(1)] public long OutputToken { get; set; }
        [Id(2)] public long TotalUsageToken { get; set; }
        [Id(3)] public long ElapsedSeconds { get; set; }
    }

    [GenerateSerializer]
    public class MaybeNewGAgentStateLogEvent : StateLogEventBase<TokenUsageProjectionStateLogEvent>
    {
        [Id(0)] public Guid GAgentPrimaryKey { get; set; }
    }
}