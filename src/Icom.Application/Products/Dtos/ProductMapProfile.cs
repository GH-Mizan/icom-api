using AutoMapper;
using Icom.Entities;

namespace Icom.Products.Dtos
{
    public class ProductMapProfile : Profile
    {
        public ProductMapProfile()
        {
            CreateMap<ProductEntryDto, Product>();
            CreateMap<Product, ProductEntryDto>();
        }
    }
}
