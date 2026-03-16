using AutoMapper;
using Icom.BtebSessions.Dtos;
using Icom.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icom.Assets.Dtos
{
    public class AssetMapProfile : Profile
    {
        public AssetMapProfile()
        {
            CreateMap<AssetEntryDto, Asset>();
            CreateMap<Asset, AssetEntryDto>();
        }
    }
}
