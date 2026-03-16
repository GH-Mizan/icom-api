using Icom.Enums;
using System;

namespace Icom.Assets.Dtos
{
    public class AssetOutputDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public AssetUsedCondition Condition { get; set; }
        public string ConditionText { get; set; }
        public AssetType Type { get; set; }
        public string TypeText { get; set; }
        public decimal Value { get; set; }
        public string Remarks { get; set; }
        public int TenantId { get; set; }
    }
}
