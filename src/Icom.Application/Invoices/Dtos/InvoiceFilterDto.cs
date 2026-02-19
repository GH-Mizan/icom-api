using Icom.Common;
using Icom.Enums;

namespace Icom.Invoices.Dtos
{
    public class InvoiceFilterDto : FilterBaseDto
    {
        public InvoiceType? InvoiceType { get; set; }
    }
}
