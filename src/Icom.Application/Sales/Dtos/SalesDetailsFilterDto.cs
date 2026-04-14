using Icom.Common;

namespace Icom.Sales.Dtos
{
    public class SalesDetailsFilterDto : FilterBaseDto
    {
        public int? ClientId { get; set; }
        public int? CategoryId { get; set; }
        public int? ProductId { get; set; }
    }
}
