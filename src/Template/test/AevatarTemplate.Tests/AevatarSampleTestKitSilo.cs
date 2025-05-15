using Aevatar.TestKit;
using Microsoft.Extensions.DependencyInjection;

namespace AevatarTemplate.Tests;

public class AevatarSampleTestKitSilo : TestKitSilo
{
    protected override void ConfigureServices(IServiceCollection services)
    {
        // Need to register instance.
        services.AddSingleton<ISampleService>(new SampleService());
    }
}