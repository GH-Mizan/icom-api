using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.UI;
using Icom.ClassSheets.Dto;
using Icom.Entities;
using Icom.Enums;
using Icom.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Icom.ClassSheets
{
    public class ClassSheetsAppService : IcomAppServiceBase, IClassSheetsAppService
    {
        private readonly IRepository<ClassSheetDistribution> _csDistributionRepository;
        private readonly IRepository<ClassSheetInventory> _csInventoryRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly IAbpSession _abpSession;
        public ClassSheetsAppService(
            IRepository<ClassSheetDistribution> csDistributionRepository,
            IRepository<ClassSheetInventory> csInventoryRepository,
            IRepository<Student> studentRepository,
            IAbpSession abpSession
            )
        {
            _csDistributionRepository = csDistributionRepository;
            _csInventoryRepository = csInventoryRepository;
            _studentRepository = studentRepository;
            _abpSession = abpSession;
            
        }

        public async Task<List<ClassSheetInventoryDto>> GetClassSheetInventoriesAsync()
        {
            var  invantories = await _csInventoryRepository.GetAllListAsync();
            var output = invantories.Select(s => new ClassSheetInventoryDto()
            {
                Id = s.Id,
                Type = s.Type,
                SheetName = s.Type.DisplayName(),
                Quantity = s.Quantity,
            }).ToList();
            foreach (ClassSheetType type in (ClassSheetType[])Enum.GetValues(typeof(ClassSheetType)))
            {
                if(!invantories.Any(x=> x.Type == type))
                {
                    var inventory = new ClassSheetInventory()
                    {
                        Type = type,
                        Quantity = 0,
                        TenantId = _abpSession.TenantId.Value
                    };
                    await _csInventoryRepository.InsertAsync(inventory);
                    output.Add(new ClassSheetInventoryDto()
                    {
                        Type = type,
                        SheetName= type.DisplayName(),
                        Quantity = 0
                    });
                } 
            }
            return output.OrderBy(o=> o.Type).ToList();
        }

        public async Task CreateUpdateSheetInventoriesAsync(ClassSheetType type, int qty) 
        {
            var entity = await _csInventoryRepository.SingleAsync(x => x.Type == type);
            entity.Quantity = qty;
            await _csInventoryRepository.UpdateAsync(entity);
        }

        [UnitOfWork]
        public async Task DistributeClassSheetAsync(ClassSheetDistributionInputDto input)
        {
            var entity = await _csDistributionRepository.FirstOrDefaultAsync(x => x.Type == input.Type && x.StudentId == input.StudentId);
            var inventory = await _csInventoryRepository.SingleAsync(x => x.Type == input.Type);
            if (entity == null)
            {
                if(inventory.Quantity > 0)
                {
                    var newEntity = new ClassSheetDistribution()
                    {
                        StudentId = input.StudentId,
                        Type = input.Type,
                        Date = input.Date,
                        Distributed = input.Distributed,
                        TenantId = _abpSession.TenantId.Value
                    };
                    await _csDistributionRepository.InsertAsync(newEntity);

                    inventory.Quantity -= 1;
                    await _csInventoryRepository.UpdateAsync(inventory);

                }
                else
                {
                    throw new UserFriendlyException($"{input.Type.DisplayName()} is out of stock");
                }
            }
            else
            {
                if(entity.Distributed != input.Distributed)
                {
                    entity.Date = input.Date;
                    entity.Distributed = input.Distributed;
                    await _csDistributionRepository.UpdateAsync(entity);
                    if (input.Distributed)
                    {
                        if (inventory.Quantity > 0)
                        {
                            inventory.Quantity -= 1;
                            await _csInventoryRepository.UpdateAsync(inventory);
                        }
                        else
                        {
                            throw new UserFriendlyException($"{input.Type.DisplayName()} is out of stock");
                        }
                    }
                    else
                    {
                        inventory.Quantity += 1;
                        await _csInventoryRepository.UpdateAsync(inventory);
                    }
                }
                
                
            }
                
        }


        public async Task<PagedResultDto<ClassSheetDistributionOutputDto>> GetPaginatedClassSheetDistributionAsync(ClassSheetDistributionsFilterDto filter)
        {
            var students = new List<Student>();
            if (filter.StudentId != null)
            {
                students = await _studentRepository.GetAllListAsync(x => filter.StudentId == x.Id);
            }
            else
            {
                students = await _studentRepository.GetAllListAsync(x => x.IsActive && !x.CourseCompleted);
            }
                
            var studentIds = students.Select(s=> s.Id).ToList();
            var distributions = await _csDistributionRepository.GetAllListAsync(x=> studentIds.Contains(x.StudentId));

            var output = new List<ClassSheetDistributionOutputDto>();
            foreach (var student in students)
            {
                foreach (ClassSheetType type in (ClassSheetType[])Enum.GetValues(typeof(ClassSheetType)))
                {
                    var distribution = distributions.FirstOrDefault(x => x.StudentId == student.Id && x.Type == type);
                    var data = new ClassSheetDistributionOutputDto();
                    if (distribution != null)
                    {
                        data.StudentId = student.Id;
                        data.StudentName = student.Name;
                        data.Date = distribution.Date;
                        data.Type = distribution.Type;
                        data.SheetName = distribution.Type.DisplayName();
                        data.Distributed = distribution.Distributed;
                        data.FromRecord = true;
                    }
                    else 
                    {
                        data.StudentId = student.Id;
                        data.StudentName = student.Name;
                        data.Date = DateTime.UtcNow;
                        data.Type = type;
                        data.SheetName = type.DisplayName();
                        data.Distributed = false;
                    }
                    output.Add(data);
                }
            }
            output = output.OrderByDescending(o => o.StudentId).ThenBy(t => t.Type).ToList();
            var list = output.Skip(filter.Skip).Take(filter.Take).ToList();
           
            return new PagedResultDto<ClassSheetDistributionOutputDto>()
            {
                Items = list,
                TotalCount = output.Count()
            };
            
        }

    }
}
