using AutoMapper;
using Icom.Entities;
using Icom.Products.Dtos;

namespace Icom.Pricelists.Dtos
{
    public class PricelistMapProfile : Profile
    {
        public PricelistMapProfile()
        {
            CreateMap<PricelistEntryDto, Pricelist>();
            CreateMap<Pricelist, PricelistEntryDto>();
        }
    }
}
