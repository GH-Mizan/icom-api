using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Icom.Enums;
using System;

namespace Icom.Entities
{
    public class Invoice: FullAuditedEntity, IMustHaveTenant
    {
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; }
        public int ClientId { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public string Remarks { get; set; }
        public decimal TotalBill { get; set; }
        public int TenantId { get; set; }
    }
}
