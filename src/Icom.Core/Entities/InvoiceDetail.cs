using Abp.Domain.Entities.Auditing;

namespace Icom.Entities
{
    public class InvoiceDetail : FullAuditedEntity
    {
        public int InvoiceId { get; set; }
        public int? ProductId { get; set; }
        public string SerialNumber { get; set; }
        public string ServiceTypes { get; set; } //Json value (service types array)
        public string WarrantyPeriod { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
