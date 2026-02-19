using AutoMapper;
using Icom.Entities;
using Icom.Products.Dtos;

namespace Icom.Clients.Dtos
{
    public class ClientMapProfile : Profile
    {
        public ClientMapProfile()
        {
            CreateMap<ClientEntryDto, Client>();
            CreateMap<Client, ClientEntryDto>();
        }
    }
}
