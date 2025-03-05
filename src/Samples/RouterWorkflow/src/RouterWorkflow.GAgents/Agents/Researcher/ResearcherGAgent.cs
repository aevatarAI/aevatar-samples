using System.ComponentModel;
using Aevatar.Core.Abstractions;
using Aevatar.GAgents.AIGAgent.Agent;
using Aevatar.GAgents.Router.GEvents;
using Microsoft.Extensions.Logging;
using RouterWorkflow.GAgents.Agents.Events;

namespace RouterWorkflow.GAgents.Agents.Researcher;

public interface IResearcherGAgent : IAIGAgent, IGAgent
{
    Task<string> GetResultAsync();
}


[Description("Research agent")]
public class ResearcherGAgent : AIGAgentBase<ResearcherState, ResearcherStateLogEvent>, IResearcherGAgent
{
    public override async Task<string> GetDescriptionAsync()
    {
        return "Research GAgent";
    }

    [EventHandler]
    public async Task HandleEventAsync(ResearchEvent @event)
    {
        Logger.LogInformation("Handle research event.");

        var promt = ResearchPromptTemplate.Prompt.Replace("{CONTENT}", @event.Content);
        var chatResult = await ChatWithHistory(promt);
        var researchResult = chatResult?[0].Content;

        RaiseEvent(new SetResearchResultStateLogEvent
        {
            ResearchResult = researchResult
        });
        await ConfirmEvents();

        await PublishAsync(new RouteNextGEvent
        {
            ProcessResult = researchResult
        });
    }

    public async Task<string> GetResultAsync()
    {
        return State.ResearchResult;
    }

    protected override void AIGAgentTransitionState(ResearcherState state,
        StateLogEventBase<ResearcherStateLogEvent> @event)
    {
        switch (@event)
        {
            case SetResearchResultStateLogEvent setResearchResultStateLogEvent:
                State.ResearchResult = setResearchResultStateLogEvent.ResearchResult;
                break;
        }
    }
}