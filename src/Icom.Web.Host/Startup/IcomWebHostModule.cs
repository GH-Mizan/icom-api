using Abp.Modules;
using Abp.Reflection.Extensions;
using Icom.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Icom.Web.Host.Startup
{
    [DependsOn(
       typeof(IcomWebCoreModule))]
    public class IcomWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public IcomWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(IcomWebHostModule).GetAssembly());
        }
    }
}
