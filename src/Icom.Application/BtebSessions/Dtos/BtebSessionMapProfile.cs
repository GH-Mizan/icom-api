using AutoMapper;
using Icom.Entities;

namespace Icom.BtebSessions.Dtos
{
    public class BtebSessionMapProfile : Profile
    {
        public BtebSessionMapProfile()
        {
            CreateMap<BtebSessionEntryInputDto, BtebSession>();
            CreateMap<BtebSession, BtebSessionEntryInputDto>();
        }
    }
}
