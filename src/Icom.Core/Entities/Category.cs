using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Icom.Entities
{
    public class Category : FullAuditedEntity, IMayHaveTenant
    {
        public string CategoryName { get; set; }
        public int? TenantId { get; set; }
    }
}
