using Icom.Configuration.Dto;
using System.Threading.Tasks;

namespace Icom.Configuration;

public interface IConfigurationAppService
{
    Task ChangeUiTheme(ChangeUiThemeInput input);
}
