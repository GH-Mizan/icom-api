using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Icom.Authorization;

namespace Icom;

[DependsOn(
    typeof(IcomCoreModule),
    typeof(AbpAutoMapperModule))]
public class IcomApplicationModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Authorization.Providers.Add<IcomAuthorizationProvider>();
    }

    public override void Initialize()
    {
        var thisAssembly = typeof(IcomApplicationModule).GetAssembly();

        IocManager.RegisterAssemblyByConvention(thisAssembly);

        Configuration.Modules.AbpAutoMapper().Configurators.Add(
            // Scan the assembly for classes which inherit from AutoMapper.Profile
            cfg => cfg.AddMaps(thisAssembly)
        );
    }
}
