using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Icom.Entities;
using Icom.Inventories.Dtos;
using Icom.Pricelists.Dtos;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Icom.Inventories
{
    public class InventoryAppService : IcomAppServiceBase, IInventoryAppService
    {
        private readonly IRepository<Inventory> _inventoryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public InventoryAppService(
            IRepository<Inventory> inventoryRepository,
            IRepository<Product> productRepository,
            IAbpSession abpSession,
            IUnitOfWorkManager unitOfWorkManager
            )
        {
            _inventoryRepository = inventoryRepository;
            _productRepository = productRepository;
            _abpSession = abpSession;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<PagedResultDto<InventoryOutputDto>> GetPaginatedAsync(InventoriesFilterDto filter)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var searchText = string.IsNullOrEmpty(filter.SearchText) ? null : filter.SearchText.ToLower().Trim();
                var query = from i in await _inventoryRepository.GetAllAsync()
                        join p in await _productRepository.GetAllAsync() on i.ProductId equals p.Id
                        where _abpSession.TenantId == null || p.TenantId == _abpSession.TenantId
                        select new InventoryOutputDto()
                        {
                            Id = i.Id,
                            ProductId = i.ProductId,
                            ProductName = p.ProductName,
                            SerialNo = i.SerialNo,
                            Quantity = i.Quantity
                        };

                if (searchText != null)
                {
                    query = query.Where(x =>
                    x.ProductName.ToLower().Trim().Contains(searchText) ||
                    x.SerialNo.ToLower().Trim().Contains(searchText)
                    );
                }

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderBy(x => x.ProductName)
                    .Skip(filter.Skip)
                    .Take(filter.Take)
                    .ToListAsync();

                return new PagedResultDto<InventoryOutputDto>(totalCount, items);
            }
        }
    }
}
