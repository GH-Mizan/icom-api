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
using Icom.Students.Dtos;
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

        public async Task<ServicesPagedResultDto> GetPaginatedServicesAsync(ServicesFilterDto filter)
        {
            var searchText = string.IsNullOrEmpty(filter.SearchText) ? null : filter.SearchText.ToLower().Trim();
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var query = from s in await _serviceRepository.GetAllAsync()
                            join c in await _clientRepository.GetAllAsync() on s.ClientId equals c.Id
                            where (_abpSession.TenantId == null || s.TenantId == _abpSession.TenantId)
                            && (filter.ClientId == null || s.ClientId == filter.ClientId)
                            select new ServiceOutputDto()
                            {
                                Id = s.Id,
                                Date = s.Date,
                                InvoiceNumber = s.InvoiceNumber,
                                ServiceTypes = s.ServiceTypes,
                                ServiceCharge = s.ServiceCharge,
                                TotalPaid = s.TotalPaid,
                                Discount = s.Discount,
                                NetServiceCharge = s.NetServiceCharge,
                                Due = s.Due,
                                ClientId = s.ClientId,
                                ClientName = c.Name,
                                ClientType = c.Type,
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

                if(!string.IsNullOrEmpty(filter.ServiceType))
                {
                    query = query.Where(x=> x.ServiceTypes.Contains(filter.ServiceType));
                }

                if (filter.LifetimeDue)
                {
                    query = query.Where(x => x.Due > 0);
                }
                else
                {
                    if (filter.DueOnly)
                    {
                        query = query.Where(x => x.Due > 0);
                    }
                    if (filter.DateRangeSearch)
                    {
                        query = query.Where(x => x.Date.Date >= filter.StartDate.Value.Date && x.Date.Date <= filter.EndDate.Value.Date);
                    }
                    else if (filter.MonthlySearch)
                    {
                        query = query.Where(x => x.Date.Year == filter.Year && x.Date.Month == filter.Month);
                    }
                }

                var output = new ServicesPagedResultDto()
                {
                    TotalServices = await query.SumAsync(s => s.ServiceCharge),
                    TotalPaid = await query.SumAsync(s => s.TotalPaid),
                    TotalDue = await query.SumAsync(s => s.Due),
                    OverallDue = await _serviceRepository.GetAll().SumAsync(s => s.Due)
                };

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.Date)
                    .Skip(filter.Skip)
                    .Take(filter.Take).ToListAsync();

                foreach (var p in items)
                {
                    p.ServiceTypeNames = "";
                    List<string> serviceTypes = p.ServiceTypes.Split(',').ToList();
                    foreach (var st in serviceTypes)
                    {
                        p.ServiceTypeNames += ((ServiceType)Convert.ToInt32(st)).DisplayName() + ", ";
                    }
                    p.PaymentStatusText = p.PaymentStatus.DisplayName();
                }

                output.Services = new PagedResultDto<ServiceOutputDto>(totalCount, items);

                return output;
            }
        }

        public async Task<ServiceEntryDto> GetAsync(int id)
        {
            var entity = await _serviceRepository.GetAsync(id);
            return ObjectMapper.Map<ServiceEntryDto>(entity);
        }

        [UnitOfWork]
        public async Task<int> CreateOrUpdateAsync(ServiceEntryInputDto input)
        {
            var id = input.Service.Id;
            if (id.HasValue)
            {
                var service = await _serviceRepository.GetAsync(id.Value);
                ObjectMapper.Map(input.Service, service);
                //await _salesRepo.UpdateAsync(sales);


                await _serviceDueReceivedHistoryRepository.DeleteAsync(x => x.ServiceId == id);
                input.DueReceived.ServiceDate = service.Date;
                input.DueReceived.ReceiveDate = service.Date;
                await InsertDueReceivedAsync(input.DueReceived);

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
                service.PaymentStatus = service.Due == 0 ? PaymentStatus.Paid : service.ServiceCharge > service.Due ? PaymentStatus.Partialpaid : PaymentStatus.Due;
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

        [UnitOfWork]
        public async Task ServiceRemoveAsync(int id)
        {
            await _serviceRepository.DeleteAsync(id);
            await _serviceDueReceivedHistoryRepository.DeleteAsync(x => x.ServiceId == id);
        }

        public List<ComboboxItemDto> GetServiceTypesSelectListAsync()
        {
            var output = ((ServiceType[])Enum.GetValues(typeof(ServiceType))).Select(c => new ComboboxItemDto() { Value = ((int)c).ToString(), DisplayText = c.DisplayName() }).ToList();
            return output;
        }

    }
}
