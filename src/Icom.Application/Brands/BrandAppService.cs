using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Icom.Brands.Dtos;
using Icom.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Icom.Brands
{
    public class BrandAppService : IcomAppServiceBase, IBrandAppService
    {
        private readonly IRepository<Brand> _brandRepository;
        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public BrandAppService(
            IRepository<Brand> brandRepository,
            IAbpSession abpSession,
            IUnitOfWorkManager unitOfWorkManager
            )
        {
            _brandRepository = brandRepository;
            _abpSession = abpSession;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<PagedResultDto<BrandOutputDto>> GetPaginatedBrandsAsync(BrandsFilterDto filter)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var query = (from c in await _brandRepository.GetAllAsync()
                             select new BrandOutputDto()
                             {
                                 Id = c.Id,
                                 BrandName = c.BrandName,
                                 ShortName = c.ShortName,
                                 TenantId = c.TenantId
                             }).AsQueryable();
                var list = query.OrderBy(o => o.BrandName).Skip(filter.Skip).Take(filter.Take).ToList();
                return new PagedResultDto<BrandOutputDto>()
                {
                    Items = list,
                    TotalCount = query.Count()
                };
            }
        }

        public async Task<BrandEntryDto> GetAsync(int id)
        {
            var entity = await _brandRepository.GetAsync(id);
            return new BrandEntryDto() { Id = entity.Id, BrandName = entity.BrandName, ShortName = entity.ShortName, TenantId = entity.TenantId };
        }

        public async Task CreateOrUpdateAsync(BrandEntryDto input)
        {
            if (input.Id.HasValue)
            {
                var b = await _brandRepository.GetAsync(input.Id.Value);
                b.BrandName = input.BrandName;
                b.ShortName = input.ShortName;
            }
            else
            {
                var entity = new Brand() { BrandName = input.BrandName, ShortName = input.ShortName, TenantId = _abpSession.TenantId };
                await _brandRepository.InsertAsync(entity);
            }
        }

        public async Task<List<ComboboxItemDto>> GetBrandsSelectListAsync()
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                return (await _brandRepository.GetAllListAsync()).Select(s => new ComboboxItemDto()
                {
                    Value = s.Id.ToString(),
                    DisplayText = s.BrandName
                }).OrderBy(o => o.DisplayText).ToList();
            }
        }
    }
}
