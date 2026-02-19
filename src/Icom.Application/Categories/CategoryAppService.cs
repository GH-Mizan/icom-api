using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using AutoMapper.Internal.Mappers;
using Icom.Categories.Dtos;
using Icom.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icom.Categories
{
    public class CategoryAppService : ApplicationService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public CategoryAppService(
            IRepository<Category> categoryRepository,
            IAbpSession abpSession,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _categoryRepository = categoryRepository;
            _abpSession = abpSession;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<PagedResultDto<CategoryOutputDto>> GetPaginatedCategoriesAsync(CategoryFilterDto filter)
        {

            //var query = _categoryRepository.GetAll()
            //    .WhereIf(_abpSession.TenantId.HasValue, x => x.TenantId == _abpSession.TenantId);
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var query = _categoryRepository.GetAll();

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderBy(x => x.CategoryName)
                    .Skip(filter.Skip)
                    .Take(filter.Take)
                    .Select(x => new CategoryOutputDto
                    {
                        Id = x.Id,
                        CategoryName = x.CategoryName,
                        TenantId = x.TenantId
                    })
                    .ToListAsync();

                return new PagedResultDto<CategoryOutputDto>(totalCount, items);
            }
        }

        public async Task<CategoryEntryDto> GetAsync(int id)
        {
            var entity = await _categoryRepository.GetAsync(id);
            return ObjectMapper.Map<CategoryEntryDto>(entity);
        }

        public async Task CreateOrUpdateAsync(CategoryEntryDto input)
        {
            if (input.Id.HasValue)
            {
                var entity = await _categoryRepository.GetAsync(input.Id.Value);
                entity.CategoryName = input.CategoryName;
            }
            else
            {
                await _categoryRepository.InsertAsync(new Category
                {
                    CategoryName = input.CategoryName,
                    TenantId = _abpSession.TenantId
                });
            }
        }

        public async Task<List<ComboboxItemDto>> GetCategoriesSelectListAsync()
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                return (await _categoryRepository.GetAllListAsync()).Select(s => new ComboboxItemDto()
                {
                    Value = s.Id.ToString(),
                    DisplayText = s.CategoryName
                }).OrderBy(o => o.DisplayText).ToList();
            }
        }
    }

}
