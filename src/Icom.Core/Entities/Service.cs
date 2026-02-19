using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Icom.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icom.Entities
{
    public class Service : FullAuditedEntity, IMustHaveTenant
    {
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; }
        public string ServiceTypes { get; set; } //Json value (service types array)
        [Column(TypeName = "decimal(10, 2)")]
        public decimal ServiceCharge { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalPaid { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Due { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public int ClientId { get; set; }
        public string Remarks { get; set; }
        public int TenantId { get; set; }

    }
}
