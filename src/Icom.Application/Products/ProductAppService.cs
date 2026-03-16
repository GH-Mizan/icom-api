using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using AutoMapper.Internal.Mappers;
using Icom.Entities;
using Icom.Products.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icom.Products
{
    public class ProductAppService : ApplicationService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Brand> _brandRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IAbpSession _abpSession;

        public ProductAppService(
            IRepository<Product> productRepository,
            IRepository<Supplier> supplierRepository,
            IRepository<Category> categoryRepository,
            IRepository<Brand> brandRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAbpSession abpSession)
        {
            _productRepository = productRepository;
            _supplierRepository = supplierRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _abpSession = abpSession;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<PagedResultDto<ProductOutputDto>> GetPaginatedAsync(ProductsFilterDto filter)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var searchText = string.IsNullOrEmpty(filter.SearchText) ? null : filter.SearchText.ToLower().Trim();
                var query = from p in await _productRepository.GetAllAsync()
                            join s in await _supplierRepository.GetAllAsync() on p.SupplierId equals s.Id
                            join c in await _categoryRepository.GetAllAsync() on p.CategoryId equals c.Id
                            join b in await _brandRepository.GetAllAsync() on p.BrandId equals b.Id into brands
                            from b in brands.DefaultIfEmpty()
                            where (_abpSession.TenantId == null || p.TenantId == _abpSession.TenantId)
                            && (filter.CategoryId == null || p.CategoryId == filter.CategoryId)
                            && (filter.BrandId == null || p.BrandId == filter.BrandId)
                            select new ProductOutputDto()
                            {
                                Id = b.Id,
                                ProductName = p.ProductName,
                                Description = p.Description,
                                CategoryId = p.CategoryId,
                                Category = c.CategoryName,
                                SupplierId = p.SupplierId,
                                Supplier = s.SupplierName,
                                BrandId = p.BrandId,
                                Brand = b.BrandName,
                                Remarks = p.Remarks
                            };
                if (searchText != null)
                {
                    query = query.Where(x =>
                    x.ProductName.ToLower().Trim().Contains(searchText) ||
                    x.Description.ToLower().Trim().Contains(searchText) ||
                    x.Brand.ToLower().Trim().Contains(searchText) ||
                    x.Category.ToLower().Trim().Contains(searchText)
                    );
                }

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderBy(x => x.ProductName)
                    .Skip(filter.Skip)
                    .Take(filter.Take)
                    .ToListAsync();

                return new PagedResultDto<ProductOutputDto>(totalCount, items);
            }
        }

        public async Task<ProductEntryDto> GetAsync(int id)
        {
            var entity = await _productRepository.GetAsync(id);
            return ObjectMapper.Map<ProductEntryDto>(entity);
        }

        public async Task CreateOrUpdateAsync(ProductEntryDto input)
        {
            if (input.Id.HasValue)
            {
                var entity = await _productRepository.GetAsync(input.Id.Value);
                ObjectMapper.Map(input, entity);
            }
            else
            {
                var entity = ObjectMapper.Map<Product>(input);
                entity.TenantId = _abpSession.TenantId.Value;
                await _productRepository.InsertAsync(entity);
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }

        public async Task<List<ComboboxItemDto>> GetProductsSelectListAsync()
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                return (await _productRepository.GetAllListAsync()).Select(s => new ComboboxItemDto()
                {
                    Value = s.Id.ToString(),
                    DisplayText = s.ProductName
                }).OrderBy(o => o.DisplayText).ToList();
            }
        }
    }

}
