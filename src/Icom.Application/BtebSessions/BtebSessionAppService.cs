using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Icom.BtebSessions.Dtos;
using Icom.Entities;
using Icom.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icom.BtebSessions
{
    public class BtebSessionAppService : IcomAppServiceBase, IBtebSessionAppService
    {
        private readonly IRepository<BtebSession> _sessionRepository;
        public BtebSessionAppService(IRepository<BtebSession> sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<PagedResultDto<BtebSessionOutputDto>> GetPaginatedBtebSessionsAsync(BtebSessionsFilterDto filter)
        {
            var searchText = string.IsNullOrEmpty(filter.SearchText) ? null : filter.SearchText.ToLower();
            var query = (from s in await _sessionRepository.GetAllAsync()
                         select new BtebSessionOutputDto()
                         {
                             Id = s.Id,
                             SessionName = s.SessionName,
                             SessionPeriod = s.SessionPeriod,
                             IsExaminationHeld = s.IsExaminationHeld,
                             ExaminationDate = s.ExaminationDate,
                             IsResultPublished = s.IsResultPublished,
                             ResultPublishedDate = s.ResultPublishedDate,
                             IsCertificateProvided = s.IsCertificateProvided,
                             CertificateDate = s.CertificateDate
                         }).AsQueryable();

            if (searchText != null)
            {
                query = query.Where(x =>
                x.SessionName.ToLower().Contains(searchText));
            }

            var btebSessions = query.OrderBy(o => o.Id).Skip(filter.Skip).Take(filter.Take).ToList();

            return new PagedResultDto<BtebSessionOutputDto>()
            {
                Items = btebSessions,
                TotalCount = query.Count()
            };
        }

        public async Task<BtebSessionEntryInputDto> GetAsync(int id)
        {
            var entity = await _sessionRepository.GetAsync(id);
            return ObjectMapper.Map<BtebSessionEntryInputDto>(entity);
        }

        public async Task CreateOrUpdateAsync(BtebSessionEntryInputDto input)
        {
            if (input.Id.HasValue)
            {
                var session = await _sessionRepository.GetAsync(input.Id.Value);
                ObjectMapper.Map(input, session);
                await _sessionRepository.UpdateAsync(session);
            }
            else
            {
                var session = ObjectMapper.Map<BtebSession>(input);
                await _sessionRepository.InsertAsync(session);
            }
        }

        public async Task<List<ComboboxItemDto>> GetBtebSessionsSelectListAsync()
        {
            var output = (await _sessionRepository.GetAllListAsync()).Select(s => new ComboboxItemDto()
            {
                Value = s.Id.ToString(),
                DisplayText = s.SessionName
            }).ToList();
            return output;
        }

    }
}
