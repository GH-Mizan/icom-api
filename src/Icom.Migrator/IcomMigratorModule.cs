using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Icom.Configuration;
using Icom.EntityFrameworkCore;
using Icom.Migrator.DependencyInjection;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;

namespace Icom.Migrator;

[DependsOn(typeof(IcomEntityFrameworkModule))]
public class IcomMigratorModule : AbpModule
{
    private readonly IConfigurationRoot _appConfiguration;

    public IcomMigratorModule(IcomEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

        _appConfiguration = AppConfigurations.Get(
            typeof(IcomMigratorModule).GetAssembly().GetDirectoryPathOrNull()
        );
    }

    public override void PreInitialize()
    {
        Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
            IcomConsts.ConnectionStringName
        );

        Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        Configuration.ReplaceService(
            typeof(IEventBus),
            () => IocManager.IocContainer.Register(
                Component.For<IEventBus>().Instance(NullEventBus.Instance)
            )
        );
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(IcomMigratorModule).GetAssembly());
        ServiceCollectionRegistrar.Register(IocManager);
    }
}
