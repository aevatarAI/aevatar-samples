using Aevatar.Core.Abstractions;
using SequentialWorkflow.GAgents.Common;

namespace SequentialWorkflow.GAgents.ProcessOrder;

[GenerateSerializer]
public class ProcessOrderEvent:EventBase
{
    [Id(0)] public UserInfo UserInfo { get; set; } 
}