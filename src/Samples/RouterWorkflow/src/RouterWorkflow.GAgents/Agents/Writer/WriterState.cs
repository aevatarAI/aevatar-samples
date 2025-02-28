using Aevatar.Core.Abstractions;
using Aevatar.GAgents.AIGAgent.State;

namespace RouterWorkflow.GAgents.Agents.Writer;

[GenerateSerializer]
public class WriterState : AIGAgentStateBase
{
    [Id(0)]
    public string Article { get; set; }
}