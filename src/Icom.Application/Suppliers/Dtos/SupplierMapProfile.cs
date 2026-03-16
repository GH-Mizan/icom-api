using AutoMapper;
using Icom.Entities;
using Icom.Sales.Dtos;

namespace Icom.Suppliers.Dtos
{
    public class SupplierMapProfile : Profile
    {
        public SupplierMapProfile()
        {
            CreateMap<SupplierEntryDto, Supplier>();
            CreateMap<Supplier, SupplierEntryDto>();

        }
    }
}
