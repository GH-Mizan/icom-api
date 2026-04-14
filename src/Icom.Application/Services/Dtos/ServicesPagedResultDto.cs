using Abp.Application.Services.Dto;

namespace Icom.Services.Dtos
{
    public class ServicesPagedResultDto
    {
        public PagedResultDto<ServiceOutputDto> Services { get; set; }
        public decimal TotalServices { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDue { get; set; }
        public decimal OverallDue { get; set; }
    }
}
