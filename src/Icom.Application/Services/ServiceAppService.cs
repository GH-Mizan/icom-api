using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.UI;
using Icom.Entities;
using Icom.Enums;
using Icom.Helpers;
using Icom.Sales.Dtos;
using Icom.Services.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icom.Services
{
    public class ServiceAppService: IcomAppServiceBase, IServiceAppService
    {
        private readonly IRepository<Service> _serviceRepository;
        private readonly IRepository<ServiceDueReceivedHistory> _serviceDueReceivedHistoryRepository;
        private readonly IRepository<Client> _clientRepository;
        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ServiceAppService(
            IRepository<Service> serviceRepository,
            IRepository<ServiceDueReceivedHistory> serviceDueReceivedHistoryRepository,
            IRepository<Client> clientRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAbpSession abpSession
            )
        {
            _serviceRepository = serviceRepository;
            _serviceDueReceivedHistoryRepository = serviceDueReceivedHistoryRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _clientRepository = clientRepository;
            _abpSession = abpSession;
        }

        public async Task<PagedResultDto<ServiceOutputDto>> GetPaginatedServicesAsync(SalesFilterDto filter)
        {
            var searchText = string.IsNullOrEmpty(filter.SearchText) ? null : filter.SearchText.ToLower().Trim();
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var query = from s in await _serviceRepository.GetAllAsync()
                            join c in await _clientRepository.GetAllAsync() on s.ClientId equals c.Id
                            where _abpSession.TenantId == null || s.TenantId == _abpSession.TenantId
                            select new ServiceOutputDto()
                            {
                                Id = s.Id,
                                Date = s.Date,
                                InvoiceNumber = s.InvoiceNumber,
                                ServiceTypes = s.ServiceTypes,
                                ServiceCharge = s.ServiceCharge,
                                TotalPaid = s.TotalPaid,
                                Due = s.Due,
                                ClientId = s.ClientId,
                                ClientName = c.Name,
                                PaymentStatus = s.PaymentStatus,
                                Remarks = s.Remarks,
                                TenantId = s.TenantId
                            };

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

                foreach (var p in items)
                {
                    p.PaymentStatusText = p.PaymentStatus.DisplayName();
                    p.ServiceTypeNames = "";
                }

                return new PagedResultDto<ServiceOutputDto>(totalCount, items);
            }
        }

        [UnitOfWork]
        public async Task<int> CreateOrUpdateAsync(ServiceEntryInputDto input)
        {
            var id = input.Service.Id;
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
                var service = ObjectMapper.Map<Service>(input.Service);
                service.TenantId = _abpSession.TenantId.Value;
                id = await _serviceRepository.InsertAndGetIdAsync(service);
                input.DueReceived.ServiceId = id.Value;
                input.DueReceived.ServiceDate = service.Date;
                input.DueReceived.ReceiveDate = service.Date;
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

        private async Task InsertDueReceivedAsync(ServiceDueReceivedHistoryDto input)
        {
            var dr = ObjectMapper.Map<ServiceDueReceivedHistory>(input);
            dr.CreationTime = DateTime.UtcNow;
            await _serviceDueReceivedHistoryRepository.InsertAsync(dr);
        }

        [UnitOfWork]
        public async Task DueReceivedEntryAsync(ServiceDueReceivedEntryDto input)
        {
            try
            {
                var service = await _serviceRepository.SingleAsync(s => s.Id == input.ServiceId);
                service.TotalPaid += input.TotalPaid;
                service.Due = input.Due;
                //sales.PaymentReceiveHistory = input.PaymentReceiveHistory;
                service.PaymentStatus = service.Due == 0 ? PaymentStatus.Paid : service.TotalPaid > service.Due ? PaymentStatus.Partialpaid : PaymentStatus.Due;
                await _serviceRepository.UpdateAsync(service);
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
                var dr = await _serviceDueReceivedHistoryRepository.SingleAsync(x => x.Id == id);
                var service = await _serviceRepository.SingleAsync(s => s.Id == dr.ServiceId);
                service.TotalPaid -= dr.TotalPaid;
                service.Due = service.ServiceCharge - service.TotalPaid;
                service.PaymentStatus = service.Due == 0 ? PaymentStatus.Paid : service.TotalPaid > service.Due ? PaymentStatus.Partialpaid : PaymentStatus.Due;
                await _serviceRepository.UpdateAsync(service);
                await _serviceDueReceivedHistoryRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<List<ServiceDueReceivedHistoryDto>> GetServiceDueReceivedHistoriesAsync(int serviceId)
        {
            try
            {
                var histories = (from d in await _serviceDueReceivedHistoryRepository.GetAllAsync()
                                 where d.ServiceId == serviceId
                                 select new ServiceDueReceivedHistoryDto()
                                 {
                                     Id = d.Id,
                                     ServiceId = d.ServiceId,
                                     ClientId = d.ClientId,
                                     ServiceDate = d.ServiceDate,
                                     ReceiveDate = d.ReceiveDate,
                                     CreationTime = d.CreationTime,
                                     PaymentStatus = d.PaymentStatus,
                                     GrandTotal = d.GrandTotal,
                                     TotalPaid = d.TotalPaid,
                                     Due = d.Due,
                                     Default = d.Default
                                 }).OrderBy(o => o.CreationTime).ToList();
                return histories;
            }
            catch (Exception ex) 
            {
                throw ex;
            }
            
        }

        public List<ComboboxItemDto> GetServiceTypesSelectListAsync()
        {
            var output = ((ServiceType[])Enum.GetValues(typeof(ServiceType))).Select(c => new ComboboxItemDto() { Value = ((int)c).ToString(), DisplayText = c.DisplayName() }).ToList();
            return output;
        }

    }
}
