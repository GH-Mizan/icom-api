using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Icom.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icom.Entities
{
    public class RegularExpense : FullAuditedEntity, IMustHaveTenant
    {
        public DateTime Date { get; set; }
        public RegularExpensePattern Pattern { get; set; }
        public RegularExpenseType Type { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public int TenantId { get; set; }
    }
}
