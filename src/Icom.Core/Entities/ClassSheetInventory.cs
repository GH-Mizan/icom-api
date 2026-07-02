using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Icom.Enums;

namespace Icom.Entities
{
    public class ClassSheetInventory : FullAuditedEntity, IMustHaveTenant
    {
        public ClassSheetType Type { get; set; }
        public int Quantity { get; set; }
        public int TenantId { get; set; }
    }
}
