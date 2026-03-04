using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace Icom.Invoices.Dtos
{
    public class InvoicedProductInfoDto
    {
        public  int ProductId { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public List<ComboboxItemDto> WarrantyPeriods { get; set; }
    }
}
