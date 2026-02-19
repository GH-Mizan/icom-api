using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Icom.Entities;
using Icom.Suppliers.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Icom.Suppliers
{
    public class SupplierAppService : IcomAppServiceBase, ISupplierAppService
    {
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IAbpSession _abpSession;

        public SupplierAppService(
            IRepository<Supplier> supplierRepository,
            IAbpSession abpSession)
        {
            _supplierRepository = supplierRepository;
            _abpSession = abpSession;
        }

        public async Task<PagedResultDto<SupplierOutputDto>> GetPaginatedAsync(SuppliersFilterDto filter)
        {
            var query = _supplierRepository.GetAll()
                .Where(x => x.TenantId == _abpSession.TenantId);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.SupplierName)
                .Skip(filter.Skip)
                .Take(filter.Take)
                .Select(x => new SupplierOutputDto()
                {
                    Id = x.Id,
                    SupplierName = x.SupplierName,
                    ShortName = x.ShortName,
                    ContactNumber = x.ContactNumber,
                    Address = x.Address
                })
                .ToListAsync();

            return new PagedResultDto<SupplierOutputDto>(totalCount, items);
        }

        public async Task<SupplierEntryDto> GetAsync(int id)
        {
            var entity = await _supplierRepository.GetAsync(id);
            return ObjectMapper.Map<SupplierEntryDto>(entity);
        }

        public async Task CreateOrUpdateAsync(SupplierEntryDto input)
        {
            if (input.Id.HasValue)
            {
                var entity = await _supplierRepository.GetAsync(input.Id.Value);
                ObjectMapper.Map(input, entity);
            }
            else
            {
                var entity = ObjectMapper.Map<Supplier>(input);
                entity.TenantId = _abpSession.TenantId.Value;
                await _supplierRepository.InsertAsync(entity);
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _supplierRepository.DeleteAsync(id);
        }

        public async Task<List<ComboboxItemDto>> GetSuppliersSelectListAsync()
        {
            return (await _supplierRepository.GetAllListAsync()).Select(s => new ComboboxItemDto()
            {
                Value = s.Id.ToString(),
                DisplayText = s.SupplierName
            }).OrderBy(o => o.DisplayText).ToList();
        }
    }

}
