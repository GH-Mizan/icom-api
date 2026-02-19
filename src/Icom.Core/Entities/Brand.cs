using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Icom.Entities
{
    public class Brand : FullAuditedEntity, IMayHaveTenant
    {
        public string BrandName { get; set; }
        public string ShortName { get; set; }
        public int? TenantId { get; set; }
    }
}
