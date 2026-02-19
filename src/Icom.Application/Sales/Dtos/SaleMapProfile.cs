using AutoMapper;
using Icom.Entities;
using Icom.Pricelists.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icom.Sales.Dtos
{
    public class SaleMapProfile : Profile
    {
        public SaleMapProfile()
        {
            CreateMap<SalesEntryDto, Sale>();
            CreateMap<Sale, SalesEntryDto>();

            CreateMap<SalesDetailsEntryDto, SaleDetail>();
            CreateMap<SaleDetail, SalesDetailsEntryDto>();

            CreateMap<DueReceivedHistory, DueReceivedHistoryDto>();
            CreateMap<DueReceivedHistoryDto, DueReceivedHistory>();
        }
    }
}
