using Icom.Common;

namespace Icom.Pricelists.Dtos
{
    public class PricelistFilterDto : FilterBaseDto
    {
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
    }
}
