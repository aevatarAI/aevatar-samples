using Aevatar.TestKit;
using AevatarTemplate.GAgents;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace AevatarTemplate.Tests;

public class SampleTests : TestKitBase<AevatarSampleTestKitSilo>
{
    [Fact(DisplayName = "Show how to register services to DI.")]
    public void TestKitDITest()
    {
        var sampleService = Silo.ServiceProvider.GetRequiredService<ISampleService>();
        sampleService.Test().ShouldNotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Show how to get a GAgent.")]
    public async Task SampleGAgentTest()
    {
        var sampleGAgent = await Silo.CreateGrainAsync<SampleGAgent>(Guid.NewGuid());
        var description = await sampleGAgent.GetDescriptionAsync();
        description.ShouldBe("This is a GAgent for sampling.");
    }
}