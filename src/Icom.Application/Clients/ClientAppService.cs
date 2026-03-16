using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Icom.Clients.Dtos;
using Icom.Entities;
using Icom.Enums;
using Icom.Helpers;
using Icom.Products.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icom.Clients
{
    public class ClientAppService: IcomAppServiceBase, IClientAppService
    {
        private readonly IRepository<Client> _clientRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IAbpSession _abpSession;
        public ClientAppService(
            IRepository<Client> clientRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAbpSession abpSession)
        {
            _clientRepository = clientRepository;
            _abpSession = abpSession;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<PagedResultDto<ClientOutputDto>> GetPaginatedClientsAsync(ClientFilterDto filter)
        {
            var searchText = string.IsNullOrEmpty(filter.SearchText) ? null : filter.SearchText.ToLower();
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var query = from c in await _clientRepository.GetAllAsync()
                            where _abpSession.TenantId == null || c.TenantId == _abpSession.TenantId
                            select new ClientOutputDto()
                            {
                                Id = c.Id,
                                EntryDate = c.EntryDate,
                                Name = c.Name,
                                IdentificationName = c.IdentificationName,
                                ContactNumber = c.ContactNumber,
                                WhatsAppNumber = c.WhatsAppNumber,
                                Email = c.Email,
                                Address = c.Address,
                                Type = c.Type,
                                TypeText = c.Type.DisplayName(),
                                Remarks = c.Remarks
                            };
                if (searchText != null)
                {
                    query = query.Where(x =>
                    x.Name.ToLower().Contains(searchText) ||
                    x.IdentificationName.ToLower().Contains(searchText) ||
                    x.ContactNumber.ToLower().Contains(searchText) ||
                    x.Address.ToLower().Contains(searchText) 
                    //x.TypeText.ToLower().Contains(searchText)
                    );
                }

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderBy(x => x.Name)
                    .Skip(filter.Skip)
                    .Take(filter.Take)
                    .ToListAsync();

                return new PagedResultDto<ClientOutputDto>(totalCount, items);
            }
        }

        public async Task<ClientEntryDto> GetAsync(int id)
        {
            var entity = await _clientRepository.GetAsync(id);
            return ObjectMapper.Map<ClientEntryDto>(entity);
        }

        public async Task CreateOrUpdateAsync(ClientEntryDto input)
        {
            if (input.Id.HasValue)
            {
                var entity = await _clientRepository.GetAsync(input.Id.Value);
                ObjectMapper.Map(input, entity);
            }
            else
            {
                var entity = ObjectMapper.Map<Client>(input);
                entity.TenantId = _abpSession.TenantId.Value;
                await _clientRepository.InsertAsync(entity);
            }
        }

        public async Task<List<ComboboxItemDto>> GetClientsSelectListAsync()
        {
            return (await _clientRepository.GetAllListAsync()).Select(s => new ComboboxItemDto()
            {
                Value = s.Id.ToString(),
                DisplayText = s.IdentificationName
            }).OrderBy(o => o.DisplayText).ToList();
        }

        public async Task<ClientInfoDto> GetClientInfoAsync(int clientId)
        {
            var client = await _clientRepository.GetAsync(clientId);
            return new ClientInfoDto()
            {
                Name = client.Name,
                IdentificationName = client.IdentificationName,
                ContactNumber = client.ContactNumber
            };
        }

        public List<ComboboxItemDto> GetClientTypesSelectListAsync()
        {
            var output = ((ClientType[])Enum.GetValues(typeof(ClientType))).Select(c => new ComboboxItemDto() { Value = ((int)c).ToString(), DisplayText = c.DisplayName() }).ToList();
            return output;
        }
    }
}
