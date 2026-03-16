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
using System.Runtime.InteropServices;
using System.Text.Json;
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
            _invoiceRepository = invoiceRepository;
            _invoiceDetailsRepository = invoiceDetailsRepository;
        }

        public async Task<PagedResultDto<SaleOutputDto>> GetPaginatedAsync(SalesFilterDto filter)
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

                if (searchText != null)
                {
                    query = query.Where(x =>
                    x.InvoiceNumber.ToLower().Trim().Contains(searchText) ||
                    x.ClientName.ToLower().Trim().Contains(searchText)
                    );
                }

                if(filter.LifetimeDue)
                {
                    query = query.Where(x => x.DueAmount > 0);
                }
                else
                {
                    if(filter.DueOnly)
                    {
                        query = query.Where(x => x.DueAmount > 0);
                    }
                    if(filter.DateRangeSearch)
                    {
                        query = query.Where(x => x.Date.Date >= filter.StartDate.Value.Date && x.Date.Date <= filter.EndDate.Value.Date);
                    } else if(filter.MonthlySearch)
                    {
                        query = query.Where(x => x.Date.Year == filter.Year && x.Date.Month == filter.Month);
                    }
                }

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.Date)
                    .Skip(filter.Skip)
                    .Take(filter.Take).ToListAsync();

                foreach (var p in items)
                {
                    p.PaymentStatusText = p.PaymentStatus.DisplayName();
                }

                return new PagedResultDto<SaleOutputDto>(totalCount, items);
            }
        }

        [UnitOfWork]
        public async Task<int> CreateOrUpdateAsync(SalesEntryInputDto input)
        {
            var id = input.Sales.Id;
            if (id.HasValue)
            {
                //var sales = await _salesRepo.GetAsync(id.Value);
                //ObjectMapper.Map(input.Sales, sales);
                //await _salesRepo.UpdateAsync(sales);

                //var prevSalesDetails = await _salesDetailsRepo.GetAllListAsync(x => x.SaleId == id);
                //foreach (var sd in prevSalesDetails)
                //{
                //    var inventory = await _inventoryRepo.FirstOrDefaultAsync(f => f.ProductId == sd.ProductId && f.StockPointId == input.Sales.StockPointId);
                //    if (inventory != null)
                //    {
                //        inventory.StockQty += sd.Quantity;
                //        await _inventoryRepo.UpdateAsync(inventory);
                //    }
                //}
                //await _salesDetailsRepo.BatchDeleteAsync(x => x.SaleId == id);
                //await InsertSalesDetails(input.SalesDetails, id.Value, input.Sales.StockPointId);

                //await _dueReceivedHistoryRepo.DeleteAsync(x => x.SalesId == id);
                //await InsertDueReceivedAsync(input.DueReceived);

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

        public async Task<SaleEntryDto> GetAsync(int id)
        {
            var entity = await _saleRepository.GetAsync(id);
            return ObjectMapper.Map<SaleEntryDto>(entity);
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
    }

}
