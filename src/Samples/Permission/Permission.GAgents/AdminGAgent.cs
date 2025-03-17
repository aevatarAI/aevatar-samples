using Aevatar.Core;
using Aevatar.Core.Abstractions;
using Aevatar.PermissionManagement;

namespace Permission.GAgents;

[GAgent(nameof(AdminGAgent))]
[Permission("Admin.Read", groupName: "Admin")]
public class AdminGAgent : GAgentBase<AdminGAgentState, AdminStateLogEvent>, IAdminGAgent
{
    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("This is AdminGAgent");
    }
    
    [Permission("Admin.Write", groupName: "Admin")]
    public async Task CreateAsync()
    {
    }

    [Permission("Admin.Write", groupName: "Admin")]
    public async Task DelAsync()
    {
    }

    
    public async Task GetAsync()
    {
    }
    
    public async Task GetListAsync()
    {
    }
}

[GenerateSerializer]
public class AdminStateLogEvent : StateLogEventBase<AdminStateLogEvent>
{
    
}

[GenerateSerializer]
public class AdminGAgentState : StateBase
{
    
}

public interface IAdminGAgent : IGAgent
{
    Task CreateAsync();
    Task DelAsync();
    Task GetAsync();

    Task GetListAsync();
}