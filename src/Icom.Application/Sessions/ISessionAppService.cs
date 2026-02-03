using Abp.Application.Services;
using Icom.Sessions.Dto;
using System.Threading.Tasks;

namespace Icom.Sessions;

public interface ISessionAppService : IApplicationService
{
    Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
}
