using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Icom.SetupExpenses;
using Icom.SetupExpenses.Dtos;
using Icom.Entities;
using System.Threading.Tasks;
using System.Linq;
using Icom.Helpers;

namespace Icom.SetupExpenses
{
    public class SetupExpenseAppService : IcomAppServiceBase, ISetupExpenseAppService
    {
        private readonly IRepository<SetupExpense> _setupExpenseRepository;
        public SetupExpenseAppService(IRepository<SetupExpense> setupExpenseRepository)
        {
            _setupExpenseRepository = setupExpenseRepository;
        }

        public async Task<PagedResultDto<SetupExpenseOutputDto>> GetPaginatedSetupExpensesAsync(SetupExpensesFilterDto filter)
        {
            var searchText = string.IsNullOrEmpty(filter.SearchText) ? null : filter.SearchText.ToLower();
            var query = (from s in await _setupExpenseRepository.GetAllAsync()
                         select new SetupExpenseOutputDto()
                         {
                             Id = s.Id,
                             Date = s.Date,
                             Purpose = s.Purpose,
                             Amount = s.Amount,
                             Remarks = s.Remarks
                         }).AsQueryable();

            if (searchText != null)
            {
                query = query.Where(x =>
                x.Remarks.ToLower().Contains(searchText));
            }

            var setupExpenses = query.OrderBy(o => o.Id).Skip(filter.Skip).Take(filter.Take).ToList();

            return new PagedResultDto<SetupExpenseOutputDto>()
            {
                Items = setupExpenses,
                TotalCount = query.Count()
            };
        }

        public async Task<SetupExpenseEntryDto> GetAsync(int id)
        {
            var entity = await _setupExpenseRepository.GetAsync(id);
            return ObjectMapper.Map<SetupExpenseEntryDto>(entity);
        }

        public async Task CreateOrUpdateAsync(SetupExpenseEntryDto input)
        {
            if (input.Id.HasValue)
            {
                var setupExpense = await _setupExpenseRepository.GetAsync(input.Id.Value);
                ObjectMapper.Map(input, setupExpense);
                await _setupExpenseRepository.UpdateAsync(setupExpense);
            }
            else
            {
                var setupExpense = ObjectMapper.Map<SetupExpense>(input);
                await _setupExpenseRepository.InsertAsync(setupExpense);
            }
        }

        public async Task SetupExpenseRemoveAsync(int id)
        {
            await _setupExpenseRepository.DeleteAsync(id);
        }

    }
}
