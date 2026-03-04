using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icom.Entities
{
    public class Due : FullAuditedEntity, IMustHaveTenant
    {
        public DateTime Date { get; set; }
        public int ClientId { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal DueAmount { get; set; }
        public string Reference { get; set; }
        public string Remarks { get; set; }
        public int TenantId { get; set; }
    }
}
