using Abp.Application.Services.Dto;

namespace Icom.Sales.Dtos
{
    public class SalesPagedResultDto
    {
        public PagedResultDto<SaleOutputDto> Sales { get; set; }
        public decimal TotalNetSales { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDue { get; set; }
        public decimal OverallDue { get; set; }
    }
}
