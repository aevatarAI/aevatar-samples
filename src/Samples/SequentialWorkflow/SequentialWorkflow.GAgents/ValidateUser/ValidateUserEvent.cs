using Aevatar.Core.Abstractions;
using SequentialWorkflow.GAgents.Common;

namespace SequentialWorkflow.GAgents.ValidateUser;

[GenerateSerializer]
public class ValidateUserEvent:EventBase
{
    [Id(0)] public UserInfo UserInfo { get; set; }
}

[GenerateSerializer]
public class ValidateUserPassEvent : EventBase
{
    [Id(0)] public UserInfo UserInfo { get; set; }
}
