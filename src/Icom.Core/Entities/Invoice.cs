using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Icom.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icom.Entities
{
    public class Invoice: FullAuditedEntity, IMustHaveTenant
    {
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; }
        public int ClientId { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public string Remarks { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalBill { get; set; }
        public int TenantId { get; set; }
    }
}
