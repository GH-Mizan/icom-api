using AutoMapper;
using Icom.Entities;

namespace Icom.SetupExpenses.Dtos
{
    public class SetupExpenseMapProfile : Profile
    {
        public SetupExpenseMapProfile()
        {
            CreateMap<SetupExpenseEntryDto, SetupExpense>();
            CreateMap<SetupExpense, SetupExpenseEntryDto>();
        }
    }
}
