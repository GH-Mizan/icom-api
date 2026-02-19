using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Icom.Entities
{
    public class Inventory : FullAuditedEntity, IMustHaveTenant
    {
        public int ProductId { get; set; }
        public string SerialNo { get; set; }
        public int Quantity { get; set; }
        public int TenantId { get; set; }
    }
}
