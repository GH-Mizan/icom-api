using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Icom.MultiTenancy;

namespace Icom.Sessions.Dto;

[AutoMapFrom(typeof(Tenant))]
public class TenantLoginInfoDto : EntityDto
{
    public string TenancyName { get; set; }

    public string Name { get; set; }
}
