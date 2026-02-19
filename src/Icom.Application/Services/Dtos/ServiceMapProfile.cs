using AutoMapper;
using Icom.Entities;
using Icom.Sales.Dtos;

namespace Icom.Services.Dtos
{
    public class ServiceMapProfile : Profile
    {
        public ServiceMapProfile()
        {
            CreateMap<ServiceEntryDto, Service>();
            CreateMap<Service, ServiceEntryDto>();

            CreateMap<ServiceDueReceivedHistory, ServiceDueReceivedHistoryDto>();
            CreateMap<ServiceDueReceivedHistoryDto, ServiceDueReceivedHistory>();
        }
    }
}
