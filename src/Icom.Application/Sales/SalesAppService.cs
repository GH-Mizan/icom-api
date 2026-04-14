using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Icom.Entities;
using Icom.Enums;
using Icom.Helpers;
using Icom.Sales.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Icom.Sales
{
    public class SalesAppService : IcomAppServiceBase, ISalesAppService
    {
        private readonly IRepository<Sale> _saleRepository;
        private readonly IRepository<SaleDetail> _saleDetailRepository;
        private readonly IRepository<DueReceivedHistory> _dueReceivedHistoryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Pricelist> _pricelistRepository;
        private readonly IRepository<Inventory> _inventoryRepository;
        private readonly IRepository<Client> _clientRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<InvoiceDetail> _invoiceDetailsRepository;
        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public SalesAppService(
            IRepository<Sale> saleRepository,
            IRepository<SaleDetail> saleDetailRepository,
            IRepository<DueReceivedHistory> dueReceivedHistoryRepository,
            IRepository<Inventory> inventoryRepository,
            IRepository<Product> productRepository,
            IRepository<Pricelist> pricelistRepository,
            IRepository<Client> clientRepository,
            IRepository<Category> categoryRepository,
            IRepository<Invoice> invoiceRepository,
            IRepository<InvoiceDetail> invoiceDetailsRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAbpSession abpSession)
        {
            _saleRepository = saleRepository;
            _saleDetailRepository = saleDetailRepository;
            _inventoryRepository = inventoryRepository;
            _abpSession = abpSession;
            _productRepository = productRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _pricelistRepository = pricelistRepository;
            _dueReceivedHistoryRepository = dueReceivedHistoryRepository;
            _clientRepository = clientRepository;
            _categoryRepository = categoryRepository;
            _invoiceRepository = invoiceRepository;
            _invoiceDetailsRepository = invoiceDetailsRepository;
        }

        public async Task<SalesPagedResultDto> GetPaginatedAsync(SalesFilterDto filter)
        {
            var searchText = string.IsNullOrEmpty(filter.SearchText) ? null : filter.SearchText.ToLower().Trim();
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var query = from s in await _saleRepository.GetAllAsync()
                        join c in await _clientRepository.GetAllAsync() on s.ClientId equals c.Id
                        where (_abpSession.TenantId == null || s.TenantId == _abpSession.TenantId)
                        && (filter.ClientId == null || s.ClientId == filter.ClientId)
                            select new SaleOutputDto()
                        {
                            Id = s.Id,
                            Date = s.Date,
                            InvoiceNumber = s.InvoiceNumber,
                            ClientId = s.ClientId,
                            ClientName = c.Name,
                            TotalAmount = s.TotalAmount,
                            Discount = s.Discount,
                            NetAmount = s.NetAmount,
                            PaidAmount = s.PaidAmount,
                            DueAmount = s.DueAmount,
                            PaymentStatus = s.PaymentStatus,
                            TenantId = s.TenantId
                        };

                if (filter.LifetimeDue)
                {
                    query = query.Where(x => x.DueAmount > 0);
                }
                else
                {
                    if (searchText != null)
                    {
                        query = query.Where(x =>
                        x.InvoiceNumber.ToLower().Trim().Contains(searchText) ||
                        x.ClientName.ToLower().Trim().Contains(searchText)
                        );
                    }

                    if(filter.IncludeDateSearch)
                    {
                        if (filter.DateRangeSearch)
                        {
                            query = query.Where(x => x.Date.Date >= filter.StartDate.Value.Date && x.Date.Date <= filter.EndDate.Value.Date);
                        }
                        else if (filter.MonthlySearch)
                        {
                            query = query.Where(x => x.Date.Year == filter.Year && x.Date.Month == filter.Month);
                        }
                    }
                }
                               
                var output = new SalesPagedResultDto()
                {
                    TotalNetSales = await query.SumAsync(s=> s.NetAmount),
                    TotalPaid = await query.SumAsync(s => s.PaidAmount),
                    TotalDue = await query.SumAsync(s => s.DueAmount),
                    OverallDue = await _saleRepository.GetAll().SumAsync(s=>s.DueAmount)
                };

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.Date)
                    .Skip(filter.Skip)
                    .Take(filter.Take).ToListAsync();

                foreach (var p in items)
                {
                    p.PaymentStatusText = p.PaymentStatus.DisplayName();
                }
                output.Sales = new PagedResultDto<SaleOutputDto>(totalCount, items);

                return output;
            }
        }

        public async Task<PagedResultDto<SalesDetailsOutputDto>> GetPaginatedSalesDetailsAsync(SalesDetailsFilterDto filter)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var searchText = string.IsNullOrEmpty(filter.SearchText) ? null : filter.SearchText.ToLower().Trim();
                var query = from sd in await _saleDetailRepository.GetAllAsync()
                            join s in await _saleRepository.GetAllAsync() on sd.SaleId equals s.Id
                            join cl in await _clientRepository.GetAllAsync() on s.ClientId equals cl.Id
                            join p in await _productRepository.GetAllAsync() on sd.ProductId equals p.Id
                            join c in await _categoryRepository.GetAllAsync() on p.CategoryId equals c.Id
                            where (filter.ProductId == null || sd.ProductId == filter.ProductId)
                            && (filter.CategoryId == null || p.CategoryId == filter.CategoryId)
                            select new SalesDetailsOutputDto()
                            {
                                Id = sd.Id,                                
                                SalesId = sd.SaleId,
                                Date = s.Date,
                                ClientName = cl.IdentificationName,
                                ProductId = sd.ProductId,
                                ProductName = p.ProductName,
                                CategoryId = c.Id,
                                CategoryName = c.CategoryName,
                                SerialNo = sd.SerialNo,
                                UnitPrice = sd.UnitPrice,
                                Quantity = sd.Quantity,
                                TotalPrice = sd.TotalPrice,
                                Remarks = sd.Remarks
                            };
                if (searchText != null)
                {
                    query = query.Where(x =>
                    x.ProductName.ToLower().Trim().Contains(searchText) ||
                    x.CategoryName.ToLower().Trim().Contains(searchText) ||
                    x.SerialNo.ToLower().Trim().Contains(searchText) ||
                    x.Remarks.ToLower().Trim().Contains(searchText)
                    );
                }

                if (filter.ClientId != null) 
                {
                    var salesIds = (await _saleRepository.GetAllAsync()).Where(x => x.ClientId == filter.ClientId).Select(s => s.Id).ToList();
                    query = query.Where(x => salesIds.Contains(x.SalesId));
                }

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderBy(x => x.ProductName)
                    .Skip(filter.Skip)
                    .Take(filter.Take)
                    .ToListAsync();

                return new PagedResultDto<SalesDetailsOutputDto>(totalCount, items);
            }
        }

        [UnitOfWork]
        public async Task<int> CreateOrUpdateAsync(SalesEntryInputDto input)
        {
            var id = input.Sales.Id;
            if (id.HasValue)
            {
                var sales = await _saleRepository.GetAsync(id.Value);
                ObjectMapper.Map(input.Sales, sales);
                await _saleRepository.UpdateAsync(sales);

                var prevSalesDetails = await _saleDetailRepository.GetAllListAsync(x => x.SaleId == id);
                foreach (var sd in prevSalesDetails)
                {
                    var inventory = await _inventoryRepository.FirstOrDefaultAsync(f => f.ProductId == sd.ProductId);
                    if (inventory != null)
                    {
                        inventory.Quantity += sd.Quantity;
                        await _inventoryRepository.UpdateAsync(inventory);
                    }
                }
                await _saleDetailRepository.BatchDeleteAsync(x => x.SaleId == id);
                await InsertSalesDetails(input.SalesDetails, id.Value);

                await _dueReceivedHistoryRepository.DeleteAsync(x => x.SalesId == id);
                await InsertDueReceivedAsync(input.DueReceived);

            }
            else
            {
                var sales = ObjectMapper.Map<Sale>(input.Sales);
                sales.TenantId = _abpSession.TenantId.Value;
                id = await _saleRepository.InsertAndGetIdAsync(sales);

                await InsertSalesDetails(input.SalesDetails, id.Value);
                input.DueReceived.SalesId = id.Value;
                await InsertDueReceivedAsync(input.DueReceived);

                //var invoiceSettings = await _lbiSettingsRepo.SingleAsync(x => x.Key == InitialSetupKey.LastSalesInvoiceNumber);
                //var lastInvoiceNumber = invoiceSettings.Value;
                //string prefix = lastInvoiceNumber.Substring(0, 1);
                //var parsedInvoiceNumber = Convert.ToInt32(lastInvoiceNumber.Remove(0, 1));

                //invoiceSettings.Value = prefix + (parsedInvoiceNumber + 1).ToString().PadLeft(5, '0');
                //await _lbiSettingsRepo.UpdateAsync(invoiceSettings);
            }

            return id.Value;
        }

        [UnitOfWork]
        public async Task DueReceivedEntryAsync(DueReceivedEntryDto input)
        {
            try
            {
                var sales = await _saleRepository.SingleAsync(s => s.Id == input.SalesId);
                sales.Discount += input.Discount;
                sales.NetAmount = input.NetTotal;
                sales.PaidAmount += input.TotalPaid;
                sales.DueAmount = input.Due;
                //sales.PaymentReceiveHistory = input.PaymentReceiveHistory;
                sales.PaymentStatus = sales.DueAmount == 0 ? PaymentStatus.Paid : sales.TotalAmount > sales.DueAmount ? PaymentStatus.Partialpaid : PaymentStatus.Due;
                await _saleRepository.UpdateAsync(sales);
                await InsertDueReceivedAsync(input.DueReceived);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [UnitOfWork]
        public async Task DueReceivedRemoveAsync(int id)
        {
            try
            {
                var dr = await _dueReceivedHistoryRepository.SingleAsync(x => x.Id == id);
                var sale = await _saleRepository.SingleAsync(s => s.Id == dr.SalesId);
                sale.Discount -= dr.Discount;
                //sale.NetAmount -= dr.NetTotal;
                sale.PaidAmount -= dr.TotalPaid;
                sale.DueAmount = sale.NetAmount - sale.Discount - sale.PaidAmount;
                sale.PaymentStatus = sale.DueAmount == 0 ? PaymentStatus.Paid : sale.TotalAmount > sale.DueAmount ? PaymentStatus.Partialpaid : PaymentStatus.Due;
                await _saleRepository.UpdateAsync(sale);
                await _dueReceivedHistoryRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<List<DueReceivedHistoryDto>> GetDueReceivedHistoriesAsync(int salesId)
        {
            var histories = (from d in await _dueReceivedHistoryRepository.GetAllAsync()
                             where d.SalesId == salesId
                             select new DueReceivedHistoryDto()
                             {
                                 Id = d.Id,
                                 SalesId = d.SalesId,
                                 ClientId = d.ClientId,
                                 CreationTime = d.CreationTime,
                                 InvoiceDate = d.InvoiceDate,
                                 ReceiveDate = d.ReceiveDate,
                                 InvoiceNumber = d.InvoiceNumber,
                                 PaymentStatus = d.PaymentStatus,
                                 GrandTotal = d.GrandTotal,
                                 Discount = d.Discount,
                                 NetTotal = d.NetTotal,
                                 TotalPaid = d.TotalPaid,
                                 Due = d.Due,
                                 Default = d.Default,
                                 Remarks = d.Remarks
                             }).OrderBy(o => o.CreationTime).ToList();
            return histories;
        }

        [UnitOfWork]
        private async Task InsertSalesDetails(List<SalesDetailsEntryDto> salesDetailsInput, int salesId)
        {
            var salesDetails = new List<SaleDetail>();
            //var purchaseDetails = await _purchaseDetailsRepo.GetAllListAsync(x => salesDetailsInput.Select(s => s.ProductId).ToList().Contains(x.ProductId));
            foreach (var sd in salesDetailsInput)
            {
                
                //if (!string.IsNullOrEmpty(sd.SerialNo))
                //{
                //    var inventory = await _inventoryRepository.FirstOrDefaultAsync(e => e.ProductId == sd.ProductId && e.SerialNo == sd.SerialNo);
                //    inventory.Quantity -= sd.Quantity;
                //}
                //else
                //{
                //    var inventory = await _inventoryRepository.SingleAsync(e => e.ProductId == sd.ProductId);
                //    inventory.Quantity -= sd.Quantity;
                //}

                var inventory = await _inventoryRepository.SingleAsync(e => e.ProductId == sd.ProductId);
                inventory.Quantity -= sd.Quantity;

                var detail = ObjectMapper.Map<SaleDetail>(sd);
                detail.SaleId = salesId;

                //var purchasePrice = purchaseDetails.Where(x => x.ProductId == sd.ProductId).First().UnitPrice;
                //detail.TotalProfit = sd.TotalPrice - (purchasePrice * sd.Quantity);
                salesDetails.Add(detail);
            }
            await _saleDetailRepository.InsertRangeAsync(salesDetails);
        }

        private async Task InsertDueReceivedAsync(DueReceivedHistoryDto input)
        {
            var dr = ObjectMapper.Map<DueReceivedHistory>(input);
            dr.TenantId = _abpSession.TenantId.Value;
            await _dueReceivedHistoryRepository.InsertAsync(dr);
        }
        //public async Task CreateOrUpdateAsync(SaleEntryDto input)
        //{
        //    var products = JsonSerializer.Deserialize<List<SaleProductDto>>(input.ProductsJson);
        //    if (input.Id.HasValue)
        //    {
        //        //var entity = await _pricelistRepository.GetAsync(input.Id.Value);
        //        //ObjectMapper.Map(input, entity);
        //    }
        //    else
        //    {
        //        foreach(var s in products)
        //        {
        //            //var entity = ObjectMapper.Map<Sale>(input);
        //            //entity.ProductId = s.ProductId;
        //            //entity.SerialNo = s.SerialNo;
        //            //entity.
        //            //entity.TenantId = _abpSession.TenantId.Value;

        //            var entity = new Sale()
        //            {
        //                ProductId = s.ProductId,
        //                SerialNo = s.SerialNo,
        //                Date = input.Date,
        //                Quantity = s.Quantity,
        //                UnitPrice = s.UnitPrice,
        //                TotalPrice = s.TotalPrice,
        //                Discount = s.Discount,
        //                NetPrice = s.NetPrice,
        //                SaleReference = "",
        //                ClientId = input.ClientId,
        //                TenantId = _abpSession.TenantId.Value
        //            };
        //            var id = await _saleRepository.InsertAndGetIdAsync(entity);
        //            var lastEntry = await _saleRepository.GetAsync(id);
        //            lastEntry.SaleReference = $"{id.ToString()}_{input.ClientId.ToString()}_{DateTime.UtcNow.Ticks.ToString()}";

        //            if(input.HasDue)
        //            {

        //            }

        //            if(input.OverallDiscount)
        //            {

        //            }

        //            if (!string.IsNullOrEmpty(s.SerialNo))
        //            {
        //                var inventory = await _inventoryRepository.FirstOrDefaultAsync(e => e.ProductId == s.ProductId && e.SerialNo == s.SerialNo);
        //                inventory.Quantity -= s.Quantity;
        //            }
        //            else
        //            {
        //                var inventory = await _inventoryRepository.SingleAsync(e => e.ProductId == s.ProductId);
        //                inventory.Quantity -= s.Quantity;
        //            }
        //        }
        //    }
        //}

        public async Task<SalesEntryInputDto> GetAsync(int id)
        {
            var output = new SalesEntryInputDto();
            var sale = await _saleRepository.GetAsync(id);
            var saleDetails = await _saleDetailRepository.GetAllListAsync(x => x.SaleId == id);

            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var details = new List<SalesDetailsEntryDto>();
                details = (from d in saleDetails
                           join p in await _productRepository.GetAllAsync() on d.ProductId equals p.Id
                           where d.SaleId == id
                           select new SalesDetailsEntryDto()
                           {
                               Id = id,
                               ProductId = d.ProductId,
                               ProductName = p.ProductName,
                               SalesId = d.SaleId,
                               SerialNo = d.SerialNo,
                               Quantity = d.Quantity,
                               UnitPrice = d.UnitPrice,
                               TotalPrice = d.TotalPrice,
                               Remarks = d.Remarks
                           }).ToList();


                return new SalesEntryInputDto()
                {
                    Sales = ObjectMapper.Map<SalesEntryDto>(sale),
                    SalesDetails = details
                };
            }
        }

        public async Task<SalesEntryInputDto> PrepareSaleFromInvoiceAsync(string invoiceNumber)
        {
            var output = new SalesEntryInputDto();
            invoiceNumber = invoiceNumber.Trim();
            var invoice = await _invoiceRepository.FirstOrDefaultAsync(f => f.InvoiceNumber == invoiceNumber && f.InvoiceType == InvoiceType.Product);
            if(invoice != null)
            {
                output.Sales = new SalesEntryDto()
                {
                    Date = invoice.Date,
                    ClientId = invoice.ClientId,
                };
                output.SalesDetails = (from id in await _invoiceDetailsRepository.GetAllAsync()
                                       join p in await _productRepository.GetAllAsync() on id.ProductId equals p.Id
                                       where id.InvoiceId == invoice.Id
                                       select new SalesDetailsEntryDto()
                                       {
                                           ProductId = id.ProductId.Value,
                                           ProductName = p.ProductName,
                                           SerialNo = id.SerialNumber,
                                           UnitPrice = id.UnitPrice.Value,
                                           Quantity = id.Quantity.Value,
                                           TotalPrice = id.TotalAmount
                                       }).ToList();

                if ((await _saleRepository.GetAllAsync()).Any(x => x.InvoiceNumber == invoiceNumber))
                    output.Sales.InvoiceNumber = "DUPLICATE";
            }
            return output;
        }

        [UnitOfWork]
        public async Task SalesRemoveAsync(int saleId)
        {
            var prevSalesDetails = await _saleDetailRepository.GetAllListAsync(x => x.SaleId == saleId);
            foreach (var sd in prevSalesDetails)
            {
                var inventory = await _inventoryRepository.FirstOrDefaultAsync(f => f.ProductId == sd.ProductId);
                if (inventory != null)
                {
                    inventory.Quantity += sd.Quantity;
                    await _inventoryRepository.UpdateAsync(inventory);
                }
            }

            await _dueReceivedHistoryRepository.BatchDeleteAsync(x => x.SalesId == saleId);
            await _saleDetailRepository.BatchDeleteAsync(x => x.SaleId == saleId);
            await _saleRepository.DeleteAsync(x => x.Id == saleId);
        }
    }

}
