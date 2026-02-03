using Abp.Application.Services;
using Icom.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace Icom.Authorization.Accounts;

public interface IAccountAppService : IApplicationService
{
    Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

    Task<RegisterOutput> Register(RegisterInput input);
}
