using Aevatar.Core;
using Aevatar.Core.Abstractions;
using Aevatar.PermissionManagement;

namespace Permission.GAgents;

[GAgent(nameof(MasterGAgent))]
public class MasterGAgent : GAgentBase<MasterGAgentState, MasterStateLogEvent>, IMasterGAgent
{
    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("This is MasterGAgent");
    }
    
    [Permission("Master.Create", groupName: "Master")]
    public async Task CreateAsync()
    {
    }

    [Permission("Master.Delete", groupName: "Master")]
    public async Task DelAsync()
    {
    }

    [Permission("Master.View", groupName: "Master")]
    public async Task GetAsync()
    {
    }
    
    [Permission("Master.View", groupName: "Master")]
    public async Task GetListAsync()
    {
    }
}

[GenerateSerializer]
public class MasterStateLogEvent : StateLogEventBase<MasterStateLogEvent>
{
    
}

[GenerateSerializer]
public class MasterGAgentState : StateBase
{
    
}

public interface IMasterGAgent : IGAgent
{
    Task CreateAsync();
    Task DelAsync();
    Task GetAsync();

    Task GetListAsync();
}