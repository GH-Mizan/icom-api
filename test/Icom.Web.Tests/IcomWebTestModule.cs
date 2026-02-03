using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Icom.EntityFrameworkCore;
using Icom.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Icom.Web.Tests;

[DependsOn(
    typeof(IcomWebMvcModule),
    typeof(AbpAspNetCoreTestBaseModule)
)]
public class IcomWebTestModule : AbpModule
{
    public IcomWebTestModule(IcomEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
    }

    public override void PreInitialize()
    {
        Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(IcomWebTestModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        IocManager.Resolve<ApplicationPartManager>()
            .AddApplicationPartsIfNotAddedBefore(typeof(IcomWebMvcModule).Assembly);
    }
}