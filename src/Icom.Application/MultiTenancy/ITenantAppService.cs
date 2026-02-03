using Abp.Application.Services;
using Icom.MultiTenancy.Dto;

namespace Icom.MultiTenancy;

public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
{
}

