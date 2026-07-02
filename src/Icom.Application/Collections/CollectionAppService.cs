using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Icom.Collections.Dto;
using Icom.Entities;
using Icom.Enums;
using Icom.Invoices.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Icom.Collections
{
    public class CollectionAppService : IcomAppServiceBase, ICollectionAppService
    {
        private readonly IRepository<DueReceivedHistory> _salesDueReveivedRepository;
        private readonly IRepository<ServiceDueReceivedHistory> _serviceDueReveivedRepository;
        public CollectionAppService(
            IRepository<DueReceivedHistory> salesDueReveivedRepository,
            IRepository<ServiceDueReceivedHistory> serviceDueReveivedRepository
            )
        {
            _salesDueReveivedRepository = salesDueReveivedRepository;
            _serviceDueReveivedRepository = serviceDueReveivedRepository;
        }

        public async Task<PagedResultDto<SalesAndServiceCollectionDto>> GetPaginatedCollectionsAsync(CollectionsFilterDto filter)
        {
            var sales = await _salesDueReveivedRepository.GetAllListAsync(x=> x.ReceiveDate.Date >= filter.StartDate.Date && x.ReceiveDate.Date <= filter.EndDate);
            var services = await _serviceDueReveivedRepository.GetAllListAsync(x => x.ReceiveDate.Date >= filter.StartDate.Date && x.ReceiveDate.Date <= filter.EndDate);

            var output = new List<SalesAndServiceCollectionDto>();
            if(filter.Type == null || filter.Type == CollectionType.Sales)
            {
                output.AddRange(sales.Select(s => new SalesAndServiceCollectionDto()
                {
                    Date = s.ReceiveDate,
                    Type = CollectionType.Sales,
                    Amount = s.TotalPaid
                }).ToList());
            }

            if(filter.Type == null || filter.Type == CollectionType.Service)
            {
                output.AddRange(services.Select(s => new SalesAndServiceCollectionDto()
                {
                    Date = s.ReceiveDate,
                    Type = CollectionType.Sales,
                    Amount = s.TotalPaid
                }).ToList());
            }


            var totalCount = output.Count();

            var items = output
                .OrderByDescending(x => x.Date)
                .Skip(filter.Skip)
                .Take(filter.Take).ToList();

            return new PagedResultDto<SalesAndServiceCollectionDto>(totalCount, items);
        }
    }
}
