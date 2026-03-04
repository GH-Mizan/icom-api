using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icom.Entities
{
    public class Pricelist : FullAuditedEntity, IMustHaveTenant
    {
        public int ProductId { get; set; }
        public string SerialNo { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string WarrantyPeriod { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PurchaseUnitPrice { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? BodyUnitPrice { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? OnlineUnitPrice { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal SaleUnitPrice { get; set; }
        public int TenantId { get; set; }
    }
}
