using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Icom.Enums;
using System;

namespace Icom.Entities
{
    public class ClassSheetDistribution: FullAuditedEntity, IMustHaveTenant
    {
        public int StudentId { get; set; }
        public DateTime Date { get; set; }
        public ClassSheetType Type { get; set; }
        public bool Distributed { get; set; }
        public int TenantId { get; set; }
    }
}
