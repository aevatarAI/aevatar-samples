using System.Diagnostics;
using Aevatar.Core;
using Aevatar.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace TokenUsageProjection.GAgents;

[GenerateSerializer]
public class TokenUsageProjectionGAgentState : StateBase
{
    [Id(0)] public long TotalUsedToken { get; set; }
    [Id(1)] public long TotalInputToken { get; set; }
    [Id(2)] public long TotalOutputToken { get; set; }
    [Id(3)] public long ElapsedSeconds { get; set; }
    [Id(4)] public HashSet<Guid> ActivatedGAgentPrimaryKeys { get; set; } = [];
    [Id(5)] public DateTime LastSnapshotTime { get; set; }
    [Id(6)] public long LastSnapshotTotalUsedToken { get; set; }
    [Id(7)] public long LastSnapshotTotalInputToken { get; set; }
    [Id(8)] public long LastSnapshotTotalOutputToken { get; set; }

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

    [EventHandler]
    public async Task HandleTakeSnapshotAsync(TakeSnapshotEvent takeSnapshotEvent)
    {
        Logger.LogInformation("HandleTakeSnapshotAsync");
        await PublishAsync(new ResponseSnapshotEvent
        {
            From = State.LastSnapshotTime,
            To = DateTime.UtcNow,
            UsedToken = State.TotalUsedToken - State.LastSnapshotTotalUsedToken,
            InputToken = State.TotalInputToken - State.LastSnapshotTotalInputToken,
            OutputToken = State.TotalOutputToken - State.LastSnapshotTotalOutputToken
        });
        RaiseEvent(new TakeSnapshotStateLogEvent());
        await ConfirmEvents();
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
            case TakeSnapshotStateLogEvent:
                State.LastSnapshotTime = DateTime.UtcNow;
                State.LastSnapshotTotalUsedToken = State.TotalUsedToken;
                State.LastSnapshotTotalInputToken = State.TotalInputToken;
                State.LastSnapshotTotalOutputToken = State.TotalOutputToken;
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
    
    [GenerateSerializer]
    public class TakeSnapshotStateLogEvent : StateLogEventBase<TokenUsageProjectionStateLogEvent>
    {
    }
}