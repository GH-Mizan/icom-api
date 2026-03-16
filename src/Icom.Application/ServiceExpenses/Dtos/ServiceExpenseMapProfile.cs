using AutoMapper;
using Icom.Entities;

namespace Icom.ServiceExpenses.Dtos
{
    public class ServiceExpenseMapProfile : Profile
    {
        public ServiceExpenseMapProfile()
        {
            CreateMap<ServiceExpenseEntryDto, ServiceExpense>();
            CreateMap<ServiceExpense, ServiceExpenseEntryDto>();
        }
    }
}
