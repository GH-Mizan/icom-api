using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icom.Entities
{
    public class SaleDetail : FullAuditedEntity
    {
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public string SerialNo { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalPrice { get; set; }
        public string Remarks { get; set; }
    }
}
