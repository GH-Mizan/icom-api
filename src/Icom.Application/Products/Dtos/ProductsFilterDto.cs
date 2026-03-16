using Icom.Common;

namespace Icom.Products.Dtos
{
    public class ProductsFilterDto : FilterBaseDto
    {
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
    }
}
