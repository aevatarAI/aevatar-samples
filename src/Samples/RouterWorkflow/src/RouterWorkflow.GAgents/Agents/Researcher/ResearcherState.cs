using Aevatar.Core.Abstractions;
using Aevatar.GAgents.AIGAgent.State;

namespace RouterWorkflow.GAgents.Agents.Researcher;

[GenerateSerializer]
public class ResearcherState : AIGAgentStateBase
{
    [Id(0)] public string ResearchResult { get; set; }
}