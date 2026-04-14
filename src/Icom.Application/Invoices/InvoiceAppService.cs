using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Runtime.Session;
using Icom.Categories.Dtos;
using Icom.Entities;
using Icom.Enums;
using Icom.Helpers;
using Icom.Invoices.Dtos;
using Icom.Roles.Dto;
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
        private readonly IRepository<Pricelist> _pricelistRepository;
        private readonly IRepository<Client> _clientRepository;

        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Brand> _brandRepository;
        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public InvoiceAppService(
            IRepository<Invoice> invoiceRepository,
            IRepository<InvoiceDetail> invoiceDetailRepository,
            IRepository<Pricelist> pricelistRepository,
            IRepository<Client> clientRepository,
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository,
            IRepository<Brand> brandRepository,
            IAbpSession abpSession,
            IUnitOfWorkManager unitOfWorkManager
            )
        {
            _invoiceRepository = invoiceRepository;
            _invoiceDetailRepository = invoiceDetailRepository;
            _pricelistRepository = pricelistRepository;
            _clientRepository = clientRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
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
                             ClientIdentificationName = c.IdentificationName,
                             InvoiceType = i.InvoiceType,
                             InvoiceTypeText = i.InvoiceType.DisplayName(),
                             Remarks = i.Remarks,
                             TotalBill = i.TotalBill
                         }).AsQueryable();

            if (searchText != null)
            {
                query = query.Where(x =>
                x.InvoiceNumber.ToLower().Trim().Contains(searchText) ||
                x.ClientName.ToLower().Trim().Contains(searchText) ||
                x.ClientIdentificationName.ToLower().Trim().Contains(searchText)
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Date)
                .Skip(filter.Skip)
                .Take(filter.Take).ToListAsync();

            return new PagedResultDto<InvoiceOutputDto>(totalCount, items);
        }

        public async Task<InvoiceEntryInputDto> GetAsync(int id)
        {
            var invoice = await _invoiceRepository.GetAsync(id);
            var invoiceDetails = await _invoiceDetailRepository.GetAllListAsync(x => x.InvoiceId == id);
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var details = new List<InvoiceDetailsEntryDto>();
                if (invoice.InvoiceType == InvoiceType.Product)
                {
                    details = (from d in invoiceDetails
                               join p in await _productRepository.GetAllAsync() on d.ProductId equals p.Id
                               join c in await _categoryRepository.GetAllAsync() on p.CategoryId equals c.Id
                               join b in await _brandRepository.GetAllAsync() on p.BrandId equals b.Id into brands
                               from b in brands.DefaultIfEmpty()
                               where d.InvoiceId == id
                               select new InvoiceDetailsEntryDto()
                               {
                                   Id = id,
                                   ProductId = d.ProductId,
                                   ProductName = p.ProductName,
                                   SerialNumber = d.SerialNumber,
                                   Category = c.CategoryName,
                                   SealText = d.SealText,
                                   Brand = b.BrandName,
                                   ServiceType = d.ServiceType,
                                   WarrantyPeriod = d.WarrantyPeriod,
                                   Quantity = d.Quantity,
                                   UnitPrice = d.UnitPrice,
                                   TotalAmount = d.TotalAmount,
                                   Uid = Guid.NewGuid()
                               }).ToList();
                }
                else
                {
                    details = (from d in invoiceDetails
                               where d.InvoiceId == id
                               select new InvoiceDetailsEntryDto()
                               {
                                   Id = id,
                                   ServiceType = d.ServiceType,
                                   ServiceTypeText = d.ServiceType.DisplayName(),
                                   WarrantyPeriod = d.WarrantyPeriod,
                                   Quantity = d.Quantity,
                                   UnitPrice = d.UnitPrice,
                                   TotalAmount = d.TotalAmount,
                                   Uid = Guid.NewGuid()
                               }).ToList();
                }


                return new InvoiceEntryInputDto()
                {
                    Invoice = ObjectMapper.Map<InvoiceEntryDto>(invoice),
                    InvoiceDetails = details
                };
            }
        }

        public async Task CreateOrUpdateInvoiceAsync(InvoiceEntryInputDto input)
        {
            var id = input.Invoice.Id;
            if(id.HasValue)
            {
                var invoice = await _invoiceRepository.GetAsync(input.Invoice.Id.Value);
                ObjectMapper.Map(input.Invoice, invoice);
                await _invoiceRepository.UpdateAsync(invoice);
                await _invoiceDetailRepository.BatchDeleteAsync(x=> x.InvoiceId == id);
            }
            else
            {
                var sales = ObjectMapper.Map<Invoice>(input.Invoice);
                sales.TenantId = _abpSession.TenantId.Value;
                id = await _invoiceRepository.InsertAndGetIdAsync(sales);
            }

            await InsertInvoiceDetails(input.InvoiceDetails, id.Value);
        }

        [UnitOfWork]
        private async Task InsertInvoiceDetails(List<InvoiceDetailsEntryDto> invoiceDetailsInput, int invoiceId)
        {
            var invoiceDetails = new List<InvoiceDetail>();
            foreach (var id in invoiceDetailsInput)
            {
                var detail = ObjectMapper.Map<InvoiceDetail>(id);
                detail.InvoiceId = invoiceId;

                invoiceDetails.Add(detail);
            }
            await _invoiceDetailRepository.InsertRangeAsync(invoiceDetails);
        }

        public async Task InvoiceRemoveAsync(int id)
        {
            await _invoiceDetailRepository.BatchDeleteAsync(x=> x.InvoiceId == id);
            await _invoiceRepository.DeleteAsync(id);
        }

        public List<ComboboxItemDto> GetInvoiceTypesSelectListAsync()
        {
            var output = ((InvoiceType[])Enum.GetValues(typeof(InvoiceType))).Select(c => new ComboboxItemDto() { Value = ((int)c).ToString(), DisplayText = c.DisplayName() }).ToList();
            return output;
        }

        public async Task<InvoicedProductInfoDto> GetWarrantyPeriodsSelectListAsync(int productId)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var info = (from p in (await _productRepository.GetAllAsync()).Where(x=> x.TenantId == _abpSession.TenantId)
                            join c in await _categoryRepository.GetAllAsync() on p.CategoryId equals c.Id
                            join b in await _brandRepository.GetAllAsync() on p.BrandId equals b.Id into brands
                            from b in brands.DefaultIfEmpty()
                            where p.Id == productId
                            select new
                            {
                                c.CategoryName,
                                b.BrandName
                            }).FirstOrDefault();
                var warrantyPeriods = (await _pricelistRepository.GetAllAsync()).Where(x => x.ProductId == productId && x.WarrantyPeriod != "" && x.WarrantyPeriod != null).Select(s => s.WarrantyPeriod).Distinct().ToList();

                return new InvoicedProductInfoDto()
                {
                    ProductId = productId,
                    Category = info.CategoryName,
                    Brand = string.IsNullOrEmpty(info.BrandName) ? "N/A" : info.BrandName,
                    WarrantyPeriods = warrantyPeriods.Select(s => new ComboboxItemDto()
                    {
                        Value = s.Trim(),
                        DisplayText = s.Trim()
                    }).ToList()
                };
            }
        }

        public async Task<string> GenerateNewInvoiceNumber()
        {
            var preNumber = (await _invoiceRepository.GetAllAsync()).OrderByDescending(x => x.Id).FirstOrDefault();
            var current = DateTime.Now.ToString("yyyyMMddHH");
            if (preNumber != null)
            {
                var newId = preNumber.Id + 1;
                return $"INV{current}#{newId}";
            }
            else
            {
                return $"INV{current}#1";
            }
        }
    }
}
