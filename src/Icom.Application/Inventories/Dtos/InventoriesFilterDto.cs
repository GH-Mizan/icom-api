using Icom.Common;

namespace Icom.Inventories.Dtos
{
    public class InventoriesFilterDto : FilterBaseDto
    {
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
    }
}
