using Aevatar.Core.Abstractions;
using Aevatar.Core.Abstractions.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TokenUsageProjection.GAgents;
using Volo.Abp;

namespace AevatarTemplate.Silo;

public class AevatarSiloHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public AevatarSiloHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var gAgentFactory = scope.ServiceProvider.GetRequiredService<IGAgentFactory>();
        var groupGAgent = await gAgentFactory.GetGAgentAsync<IStateGAgent<GroupGAgentState>>();
        var aiGAgent =
            await gAgentFactory.GetGAgentAsync<IStateGAgent<SampleAIGAgentState>>("test".ToGuid());
        var projectionGAgent =
            await gAgentFactory.GetGAgentAsync<IStateGAgent<TokenUsageProjectionGAgentState>>("test".ToGuid());
        await groupGAgent.RegisterAsync(aiGAgent);
        await groupGAgent.RegisterAsync(projectionGAgent);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}