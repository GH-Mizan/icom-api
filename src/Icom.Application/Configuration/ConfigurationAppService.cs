using Abp.Authorization;
using Abp.Runtime.Session;
using Icom.Configuration.Dto;
using System.Threading.Tasks;

namespace Icom.Configuration;

[AbpAuthorize]
public class ConfigurationAppService : IcomAppServiceBase, IConfigurationAppService
{
    public async Task ChangeUiTheme(ChangeUiThemeInput input)
    {
        await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
    }
}
