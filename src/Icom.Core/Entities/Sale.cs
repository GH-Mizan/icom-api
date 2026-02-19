using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Icom.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icom.Entities
{
    public class Sale : FullAuditedEntity, IMustHaveTenant
    {
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; }
        public int ClientId { get; set; }
        public decimal TotalAmount { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Discount { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal NetAmount { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PaidAmount { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal DueAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public int TenantId { get; set; }
       
    }
}
