using Aevatar.Core;
using Aevatar.Core.Abstractions;
using Aevatar.PermissionManagement;

namespace Permission.GAgents;

[GAgent(nameof(DeveloperGAgent))]
public class DeveloperGAgent : GAgentBase<DeveloperGAgentState, DeveloperStateLogEvent>, IDeveloperGAgent
{
    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("This is DeveloperGAgent");
    }
    
    [Permission("Developer.Create", groupName: "Developer")]
    public async Task CreateAsync()
    {
    }

    [Permission("Developer.Delete", groupName: "Developer")]
    public async Task DelAsync()
    {
    }

    [Permission("Developer.View", groupName: "Developer")]
    public async Task GetAsync()
    {
    }
    
    [Permission("Developer.View", groupName: "Developer")]
    public async Task GetListAsync()
    {
    }
}

[GenerateSerializer]
public class DeveloperStateLogEvent : StateLogEventBase<DeveloperStateLogEvent>
{
    
}

[GenerateSerializer]
public class DeveloperGAgentState : StateBase
{
    
}

public interface IDeveloperGAgent : IGAgent
{
    Task CreateAsync();
    Task DelAsync();
    Task GetAsync();

    Task GetListAsync();
}