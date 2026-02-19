using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Icom.Entities
{
    public class Product : FullAuditedEntity, IMustHaveTenant
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int? BrandId { get; set; }
        public int SupplierId { get; set; }
        public string Remarks { get; set; }
        public int TenantId { get; set; }
    }
}
