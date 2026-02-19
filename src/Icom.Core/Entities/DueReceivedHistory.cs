using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Icom.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icom.Entities
{
    public class DueReceivedHistory : FullAuditedEntity, IMustHaveTenant
    {
        public int SalesId { get; set; }
        public int ClientId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime ReceiveDate { get; set; }
        public string InvoiceNumber { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal GrandTotal { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Discount { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal NetTotal { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalPaid { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Due { get; set; }
        public bool Default { get; set; } //means first item while insert first time.
        public string Remarks { get; set; }
        public int TenantId { get; set; }
    }
}
