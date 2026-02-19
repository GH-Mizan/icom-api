using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Icom.Entities;
using Icom.Pricelists.Dtos;
using Icom.Products.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icom.Pricelists
{
    public class PricelistAppService : IcomAppServiceBase, IPricelistAppService
    {
        private readonly IRepository<Pricelist> _pricelistRepository;
        private readonly IRepository<Inventory> _inventoryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IAbpSession _abpSession;
        public PricelistAppService(
            IRepository<Pricelist> pricelistRepository,
            IRepository<Inventory> inventoryRepository,
            IRepository<Product> productRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAbpSession abpSession
            )
        {
            _pricelistRepository = pricelistRepository;
            _inventoryRepository = inventoryRepository;
            _productRepository = productRepository;
            _abpSession = abpSession;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<PagedResultDto<PricelistOutputDto>> GetPaginatedAsync(PricelistFilterDto filter)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var searchText = string.IsNullOrEmpty(filter.SearchText) ? null : filter.SearchText.ToLower().Trim();

                var query = from pl in await _pricelistRepository.GetAllAsync()
                            join p in await _productRepository.GetAllAsync() on pl.ProductId equals p.Id
                            where _abpSession.TenantId == null || p.TenantId == _abpSession.TenantId
                            select new PricelistOutputDto()
                            {
                                Id = pl.Id,
                                ProductId = pl.ProductId,
                                ProductName = p.ProductName,
                                SerialNo = pl.SerialNo,
                                PurchaseDate = pl.PurchaseDate,
                                WarrantyPeriod = pl.WarrantyPeriod,
                                Quantity = pl.Quantity,
                                PurchaseUnitPrice = pl.PurchaseUnitPrice,
                                BodyUnitPrice = pl.BodyUnitPrice,
                                OnlineUnitPrice = pl.OnlineUnitPrice,
                                SaleUnitPrice = pl.SaleUnitPrice
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

                return new PagedResultDto<PricelistOutputDto>(totalCount, items);
            }
        }

        public async Task<PricelistEntryDto> GetAsync(int id)
        {
            var entity = await _pricelistRepository.GetAsync(id);
            return ObjectMapper.Map<PricelistEntryDto>(entity);
        }

        public async Task CreateOrUpdateAsync(PricelistEntryDto input)
        {
            if (input.Id.HasValue)
            {
                //var entity = await _pricelistRepository.GetAsync(input.Id.Value);
                //ObjectMapper.Map(input, entity);
            }
            else
            {
                var entity = ObjectMapper.Map<Pricelist>(input);
                entity.TenantId = _abpSession.TenantId.Value;
                await _pricelistRepository.InsertAsync(entity);

                if(!string.IsNullOrEmpty(input.SerialNo))
                {
                    var inventory = await _inventoryRepository.FirstOrDefaultAsync(e => e.ProductId == input.ProductId && e.SerialNo == input.SerialNo);
                    if(inventory != null)
                    {
                        inventory.Quantity += input.Quantity;
                    }
                    else
                    {
                        inventory = new Inventory()
                        {
                            ProductId = input.ProductId,
                            SerialNo = input.SerialNo,
                            Quantity = input.Quantity,
                            TenantId = _abpSession.TenantId.Value
                        };
                        await _inventoryRepository.InsertAsync(inventory);
                    }
                }
                else
                {
                    var inventory = await _inventoryRepository.FirstOrDefaultAsync(e => e.ProductId == input.ProductId);
                    if (inventory != null)
                    {
                        inventory.Quantity += input.Quantity;
                    }
                    else
                    {
                        inventory = new Inventory()
                        {
                            ProductId = input.ProductId,
                            Quantity = input.Quantity,
                            TenantId = _abpSession.TenantId.Value
                        };
                        await _inventoryRepository.InsertAsync(inventory);
                    }
                }
            }
        }

        public async Task<ProductQuantityInfoDto> GetProductSerialsAsync(int productId)
        {
            var hasSerial = await (_pricelistRepository.GetAll()).AnyAsync(x=> x.ProductId == productId && x.SerialNo != null && x.SerialNo != "");
            var output = new ProductQuantityInfoDto() { HasSerial = hasSerial };
            if (!hasSerial)
            {
                output.AvailableQuantity = (await _inventoryRepository.FirstOrDefaultAsync(x => x.ProductId == productId))?.Quantity ?? 0;
            }
            else
            {
                output.Serials = (await _pricelistRepository.GetAllAsync()).Where(x => x.ProductId == productId).Select(s => s.SerialNo).ToList();
            }
            return output;
        }

        public async Task<int> GetQuantityAsync(int productId, string serialNo)
        {
            var inventory = await _inventoryRepository.FirstOrDefaultAsync(f=> f.ProductId == productId && f.SerialNo == serialNo);
            return inventory.Quantity;
        }

        public async Task<List<ComboboxItemDto>> GetProductsSelectListAsync()
        {
            return (from p in await _productRepository.GetAllAsync()
                    join pl in await _pricelistRepository.GetAllAsync() on p.Id equals pl.ProductId
                    select new ComboboxItemDto()
                    {
                        Value = p.Id.ToString(),
                        DisplayText = p.ProductName
                    }).ToList().GroupBy(t => t.Value).Select(g => new ComboboxItemDto() { Value = g.First().Value, DisplayText = g.First().DisplayText }).ToList();

        }

    }
}
