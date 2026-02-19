using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Icom.Entities
{
    public class Supplier : FullAuditedEntity, IMustHaveTenant
    {
        public string SupplierName { get; set; }
        public string ShortName { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public int TenantId { get; set; }
    }
}
