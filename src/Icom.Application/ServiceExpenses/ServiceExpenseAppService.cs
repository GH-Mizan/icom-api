using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Icom.Entities;
using Icom.Enums;
using Icom.Helpers;
using Icom.ServiceExpenses;
using Icom.ServiceExpenses.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icom.ServiceExpenses
{
    public class ServiceExpenseAppService : IcomAppServiceBase, IServiceExpenseAppService
    {
        private readonly IRepository<ServiceExpense> _serviceExpenseRepository;
        public ServiceExpenseAppService(IRepository<ServiceExpense> serviceExpenseRepository)
        {
            _serviceExpenseRepository = serviceExpenseRepository;
        }

        public async Task<PagedResultDto<ServiceExpenseOutputDto>> GetPaginatedServiceExpensesAsync(ServiceExpensesFilterDto filter)
        {
            var searchText = string.IsNullOrEmpty(filter.SearchText) ? null : filter.SearchText.ToLower();
            var query = (from s in await _serviceExpenseRepository.GetAllAsync()
                         select new ServiceExpenseOutputDto()
                         {
                             Id = s.Id,
                             Date = s.Date,
                             Type = s.Type,
                             TypeText = s.Type.DisplayName(),
                             Amount = s.Amount,
                             Remarks = s.Remarks
                         }).AsQueryable();

            if (searchText != null)
            {
                query = query.Where(x =>
                x.Remarks.ToLower().Contains(searchText));
            }

            var serviceExpenses = query.OrderBy(o => o.Id).Skip(filter.Skip).Take(filter.Take).ToList();

            return new PagedResultDto<ServiceExpenseOutputDto>()
            {
                Items = serviceExpenses,
                TotalCount = query.Count()
            };
        }

        public async Task<ServiceExpenseEntryDto> GetAsync(int id)
        {
            var entity = await _serviceExpenseRepository.GetAsync(id);
            return ObjectMapper.Map<ServiceExpenseEntryDto>(entity);
        }

        public async Task CreateOrUpdateAsync(ServiceExpenseEntryDto input)
        {
            if (input.Id.HasValue)
            {
                var serviceExpense = await _serviceExpenseRepository.GetAsync(input.Id.Value);
                ObjectMapper.Map(input, serviceExpense);
                await _serviceExpenseRepository.UpdateAsync(serviceExpense);
            }
            else
            {
                var serviceExpense = ObjectMapper.Map<ServiceExpense>(input);
                await _serviceExpenseRepository.InsertAsync(serviceExpense);
            }
        }

        public async Task ServiceExpenseRemoveAsync(int id)
        {
            await _serviceExpenseRepository.DeleteAsync(id);
        }

        public List<ComboboxItemDto> GetServiceExpenseTypeSelectListAsync()
        {
            var output = ((ServiceExpenseType[])Enum.GetValues(typeof(ServiceExpenseType))).Select(c => new ComboboxItemDto() { Value = ((int)c).ToString(), DisplayText = c.DisplayName() }).ToList();
            return output;
        }
    }
}
