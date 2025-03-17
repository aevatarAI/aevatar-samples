using Aevatar.Core.Abstractions;
using Aevatar.PermissionManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Volo.Abp;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Identity.MongoDB;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.Identity;

namespace Permission.Silo;

[DependsOn(
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpAutofacModule),
    typeof(AbpAutoMapperModule),
    typeof(AevatarPermissionManagementModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpIdentityMongoDbModule),
    typeof(AbpPermissionManagementDomainIdentityModule)
    )]
public class PermissionGAgentTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<PermissionGAgentTestModule>(); });
        // Configure<PermissionManagementOptions>(options =>
        // {
        //     options.ManagementProviders = 
        // });
        context.Services.AddHostedService<PermissionGAgentTestHostedService>();
        context.Services.AddSerilog(_ => {},
            true, writeToProviders: true);
        context.Services.AddHttpClient();
        context.Services.AddSingleton<IEventDispatcher, DefaultEventDispatcher>();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
    }
}