using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Icom.Assets.Dtos;
using Icom.Entities;
using Icom.Enums;
using Icom.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Icom.Assets
{
    public class AssetAppService : IcomAppServiceBase, IAssetAppService
    {
        private readonly IRepository<Asset> _assetRepository;
        public AssetAppService(IRepository<Asset> assetRepository)
        {
            _assetRepository = assetRepository;
        }

        public async Task<PagedResultDto<AssetOutputDto>> GetPaginatedAssetsAsync(AssetsFilterDto filter)
        {
            var searchText = string.IsNullOrEmpty(filter.SearchText) ? null : filter.SearchText.ToLower();
            var query = (from s in await _assetRepository.GetAllAsync()
                         select new AssetOutputDto()
                         {
                             Id = s.Id,
                             Date = s.Date,
                             Name = s.Name,
                             Description = s.Description,
                             Condition = s.Condition,
                             ConditionText = s.Condition.DisplayName(),
                             Type = s.Type,
                             TypeText = s.Type.DisplayName(),
                             Value = s.Value,
                             Remarks = s.Remarks
                         }).AsQueryable();

            if (searchText != null)
            {
                query = query.Where(x =>
                x.Name.ToLower().Contains(searchText));
            }

            var assets = query.OrderBy(o => o.Id).Skip(filter.Skip).Take(filter.Take).ToList();

            return new PagedResultDto<AssetOutputDto>()
            {
                Items = assets,
                TotalCount = query.Count()
            };
        }

        public async Task<AssetEntryDto> GetAsync(int id)
        {
            var entity = await _assetRepository.GetAsync(id);
            return ObjectMapper.Map<AssetEntryDto>(entity);
        }

        public async Task CreateOrUpdateAsync(AssetEntryDto input)
        {
            if (input.Id.HasValue)
            {
                var asset = await _assetRepository.GetAsync(input.Id.Value);
                ObjectMapper.Map(input, asset);
                await _assetRepository.UpdateAsync(asset);
            }
            else
            {
                var asset = ObjectMapper.Map<Asset>(input);
                await _assetRepository.InsertAsync(asset);
            }
        }

        public async Task AssetRemoveAsync(int id)
        {
            await _assetRepository.DeleteAsync(id);
        }

        public List<ComboboxItemDto> GetUsedConditionSelectListAsync()
        {
            var output = ((AssetUsedCondition[])Enum.GetValues(typeof(AssetUsedCondition))).Select(c => new ComboboxItemDto() { Value = ((int)c).ToString(), DisplayText = c.DisplayName() }).ToList();
            return output;
        }

        public List<ComboboxItemDto> GetAssetTypeSelectListAsync()
        {
            var output = ((AssetType[])Enum.GetValues(typeof(AssetType))).Select(c => new ComboboxItemDto() { Value = ((int)c).ToString(), DisplayText = c.DisplayName() }).ToList();
            return output;
        }

    }
}
