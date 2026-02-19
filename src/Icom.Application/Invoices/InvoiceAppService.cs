using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Runtime.Session;
using Icom.Categories.Dtos;
using Icom.Entities;
using Icom.Helpers;
using Icom.Invoices.Dtos;
using Icom.Sales.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icom.Invoices
{
    public class InvoiceAppService : IcomAppServiceBase, IInvoiceAppService
    {
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<InvoiceDetail> _invoiceDetailRepository;
        private readonly IRepository<Client> _clientRepository;
        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public InvoiceAppService(
            IRepository<Invoice> invoiceRepository,
            IRepository<InvoiceDetail> invoiceDetailRepository,
            IRepository<Client> clientRepository,
            IAbpSession abpSession,
            IUnitOfWorkManager unitOfWorkManager
            )
        {
            _invoiceRepository = invoiceRepository;
            _invoiceDetailRepository = invoiceDetailRepository;
            _clientRepository = clientRepository;
            _abpSession = abpSession;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task<PagedResultDto<InvoiceOutputDto>> GetPaginatedInvoicesAsync(InvoiceFilterDto filter)
        {
            var searchText = string.IsNullOrEmpty(filter.SearchText) ? null : filter.SearchText.ToLower().Trim();
            var query = (from i in await _invoiceRepository.GetAllAsync()
                         join c in await _clientRepository.GetAllAsync() on i.ClientId equals c.Id
                         where filter.InvoiceType == null || i.InvoiceType == filter.InvoiceType
                         select new InvoiceOutputDto()
                         {
                             Id = i.Id,
                             InvoiceNumber = i.InvoiceNumber,
                             Date = i.Date,
                             ClientId = i.ClientId,
                             ClientName = c.Name,
                             InvoiceType = i.InvoiceType,
                             InvoiceTypeText = i.InvoiceType.DisplayName(),
                             Remarks = i.Remarks,
                             TotalBill = i.TotalBill
                         }).AsQueryable();

            if (searchText != null)
            {
                query = query.Where(x =>
                x.InvoiceNumber.ToLower().Trim().Contains(searchText) ||
                x.ClientName.ToLower().Trim().Contains(searchText)
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Date)
                .Skip(filter.Skip)
                .Take(filter.Take).ToListAsync();

            return new PagedResultDto<InvoiceOutputDto>(totalCount, items);
        }

        //public async Task<InvoiceEntryDto> GetAsync(int id)
        //{

        //}

        public async Task CreateOrUpdateInvoiceAsync(InvoiceEntryDto input)
        {

        }

        public async Task InvoiceRemoveAsync(int id)
        {
            await _invoiceDetailRepository.BatchDeleteAsync(x=> x.InvoiceId == id);
            await _invoiceRepository.DeleteAsync(id);
        }
    }
}
