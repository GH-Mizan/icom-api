using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Icom.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icom.Entities
{
    public class Asset : FullAuditedEntity, IMustHaveTenant
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public AssetUsedCondition Condition { get; set; }
        public AssetType Type { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Value { get; set; }
        public int TenantId { get; set; }
    }
}
