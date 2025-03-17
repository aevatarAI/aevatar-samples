using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Aevatar.Core.Abstractions;
using Aevatar.Extensions;
using Aevatar.PermissionManagement.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Permission.Silo;
using Serilog;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(configuration)
    .WriteTo.Console()
    .CreateLogger();

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) => { services.AddApplication<PermissionGAgentTestModule>(); })
    .UseOrleans(silo =>
    {
        silo.AddMemoryGrainStorage("Default")
            .AddMemoryStreams(AevatarCoreConstants.StreamProvider)
            .AddMemoryGrainStorage("PubSubStore")
            .AddLogStorageBasedLogConsistencyProvider("LogStorage")
            .UseLocalhostClustering()
            .UseMongoDBClient("mongodb://localhost:27017/?maxPoolSize=555")
            .ConfigureLogging(logging => logging.AddConsole())
            .UseAevatar()
            .UseAevatarPermissionManagement();
    })
    .UseAutofac()
    .UseConsoleLifetime();

using var host = builder.Build();

await host.RunAsync();