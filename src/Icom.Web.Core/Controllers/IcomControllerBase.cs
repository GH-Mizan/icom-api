using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Icom.Controllers
{
    public abstract class IcomControllerBase : AbpController
    {
        protected IcomControllerBase()
        {
            LocalizationSourceName = IcomConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
