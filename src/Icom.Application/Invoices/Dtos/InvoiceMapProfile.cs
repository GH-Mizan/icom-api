using AutoMapper;
using Icom.Entities;
using Icom.Products.Dtos;

namespace Icom.Invoices.Dtos
{
    public class InvoiceMapProfile : Profile
    {
        public InvoiceMapProfile()
        {
            CreateMap<InvoiceEntryDto, Invoice>();
            CreateMap<Invoice, InvoiceEntryDto>();

            CreateMap<InvoiceDetailsEntryDto, InvoiceDetail>();
            CreateMap<InvoiceDetail, InvoiceDetailsEntryDto>();
        }
    }
}
