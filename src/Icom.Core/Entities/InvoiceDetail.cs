using Abp.Domain.Entities.Auditing;
using Icom.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icom.Entities
{
    public class InvoiceDetail : FullAuditedEntity
    {
        public int InvoiceId { get; set; }
        public int? ProductId { get; set; }
        public string SerialNumber { get; set; }
        public string SealText { get; set; }
        public ServiceType ServiceType { get; set; }
        public string WarrantyPeriod { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }
    }
}
