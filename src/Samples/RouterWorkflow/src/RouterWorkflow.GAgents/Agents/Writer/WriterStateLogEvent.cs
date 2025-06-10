using Aevatar.Core.Abstractions;

namespace RouterWorkflow.GAgents.Agents.Writer;

[GenerateSerializer]
public class WriterStateLogEvent: StateLogEventBase<WriterStateLogEvent>
{
    
}

[GenerateSerializer]
public class SetArticleStateLogEvent: WriterStateLogEvent
{
    [Id(0)]
    public string Article { get; set; }
}