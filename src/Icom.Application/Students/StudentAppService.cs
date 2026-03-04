using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Icom.BtebSessions.Dtos;
using Icom.Clients.Dtos;
using Icom.Entities;
using Icom.Enums;
using Icom.Helpers;
using Icom.Students.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Icom.Students
{
    public class StudentAppService: IcomAppServiceBase, IStudentAppService
    {
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<Client> _clientRepository;
        private readonly IRepository<BtebSession> _btebSessionRepository; 
        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public StudentAppService(
            IRepository<Student> studentRepository,
            IRepository<Client> clientRepository,
            IRepository<BtebSession> btebSessionRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAbpSession abpSession)
        {
            _studentRepository = studentRepository;
            _clientRepository = clientRepository;
            _btebSessionRepository = btebSessionRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _abpSession = abpSession;
        }

        public async Task<PagedResultDto<StudentOutputDto>> GetPaginatedStudentsAsync(StudentsFilterDto filter)
        {
            var searchText = string.IsNullOrEmpty(filter.SearchText) ? null : filter.SearchText.ToLower();
            var query = (from s in await _studentRepository.GetAllAsync()
                         join bs in await _btebSessionRepository.GetAllAsync() on s.BtebSessionId equals bs.Id into sessions
                         from bs in sessions.DefaultIfEmpty()
                         select new StudentOutputDto()
                         {
                             Id = s.Id,
                             Name = s.Name,
                             AdmisionDate = s.AdmisionDate,
                             FathersName = s.FathersName,
                             MothersName = s.MothersName,
                             DateOfBirth = s.DateOfBirth,
                             BloodGroup = s.BloodGroup,
                             BloodGroupText = s.BloodGroup == null ? "" : s.BloodGroup.DisplayName(),
                             ContactNumber = s.ContactNumber,
                             Email = s.Email,
                             PermanentAddress = s.PermanentAddress,
                             PresentAddress = s.PresentAddress,
                             Course = s.Course,
                             CourseName = s.Course == null ? "" : s.Course.DisplayName(),
                             Duration = s.Duration,
                             DurationText = s.Duration.DisplayName(),
                             BtebSessionId = s.BtebSessionId,
                             BtebSessionText = s.BtebSessionId == null ? "" : bs.SessionName,
                             Bteb = s.Bteb,
                             ClassRoll = s.ClassRoll,
                             BtebAdmitted = s.BtebAdmitted,
                             BtebRegistered = s.BtebRegistered,
                             BtebRegistrationNumber = s.BtebRegistrationNumber,
                             DidExam = s.DidExam,
                             ResultStatus = s.ResultStatus,
                             ResultStatusText = s.ResultStatus == null ? "" : s.ResultStatus.DisplayName(),
                             IsSessionChanged = s.IsSessionChanged,
                             Remarks = s.Remarks,
                             Result = s.Result,
                             CourseFee = s.CourseFee,
                             Discount = s.Discount,
                             RunningProgram = s.RunningProgram,
                             RunningProgramText = s.RunningProgram.DisplayName(),
                             IsActive = s.IsActive,
                             TenantId = s.TenantId
                         }).AsQueryable();

            if (searchText != null)
            {
                query = query.Where(x =>
                x.Name.ToLower().Contains(searchText));
            }

            var students = query.OrderBy(o => o.Id).Skip(filter.Skip).Take(filter.Take).ToList();

            return new PagedResultDto<StudentOutputDto>()
            {
                Items = students,
                TotalCount = query.Count()
            };
        }

        public async Task<StudentEntryInputDto> GetAsync(int id)
        {
            var entity = await _studentRepository.GetAsync(id);
            return ObjectMapper.Map<StudentEntryInputDto>(entity);
        }

        [UnitOfWork]
        public async Task CreateOrUpdateAsync(StudentEntryInputDto input)
        {
            if (input.Id.HasValue)
            {
                var student = await _studentRepository.GetAsync(input.Id.Value);

                var client = await _clientRepository.GetAsync(student.ClientId);
                client.EntryDate = input.AdmisionDate;
                client.Name = input.Name;
                client.ContactNumber = input.ContactNumber;
                client.WhatsAppNumber = input.ContactNumber;
                client.Email = input.Email;
                client.Address = input.PresentAddress;
                client.Type = ClientType.Student;
                client.Remarks = input.Remarks;

                int? lastRoll = 0;
                if (student.Bteb != input.Bteb || student.BtebSessionId != input.BtebSessionId)
                {
                    if(input.Bteb) //that means not bteb to bteb
                    {
                        lastRoll = (await _studentRepository.GetAllAsync()).OrderByDescending(o => o.ClassRoll).Where(x => x.Bteb && x.BtebSessionId == input.BtebSessionId).FirstOrDefault()?.ClassRoll;
                    }
                    else //that means bteb to not bteb
                    {
                        lastRoll = (await _studentRepository.GetAllAsync()).OrderByDescending(o => o.ClassRoll).Where(x => !x.Bteb).FirstOrDefault()?.ClassRoll;
                    }

                    input.Remarks = $"{input.Remarks} /##/ Prev Session Id: {student.BtebSessionId}; Prev Class Roll: {student.ClassRoll}";  
                }

                ObjectMapper.Map(input, student);
                //await _studentRepository.UpdateAsync(student);
            }
            else
            {
                var client = new Client()
                {
                    EntryDate = input.AdmisionDate,
                    Name = input.Name,
                    ContactNumber = input.ContactNumber,
                    WhatsAppNumber = input.ContactNumber,
                    Email = input.Email,
                    Address = input.PresentAddress,
                    Type = ClientType.Student,
                    Remarks = input.Remarks,
                    TenantId = _abpSession.TenantId.Value
                };
                var clientId = await _clientRepository.InsertAndGetIdAsync(client);
                
                var student = ObjectMapper.Map<Student>(input);
                student.ClientId = clientId;
                student.TenantId = _abpSession.TenantId.Value;

                int? lastRoll = 0;
                if (student.Bteb)
                {
                    lastRoll = (await _studentRepository.GetAllAsync()).OrderByDescending(o => o.ClassRoll).Where(x => x.Bteb && x.BtebSessionId == student.BtebSessionId).FirstOrDefault()?.ClassRoll;
                }
                else
                {
                    lastRoll = (await _studentRepository.GetAllAsync()).OrderByDescending(o => o.ClassRoll).Where(x => !x.Bteb).FirstOrDefault()?.ClassRoll;
                }
                student.ClassRoll = (lastRoll ?? 0) + 1;
                await _studentRepository.InsertAsync(student);
            }
        }

        public List<ComboboxItemDto> GetIccCoursesSelectListAsync()
        {
            var output = ((IccCourses[])Enum.GetValues(typeof(IccCourses))).Select(c => new ComboboxItemDto() { Value = ((int)c).ToString(), DisplayText = c.DisplayName() }).ToList();
            return output;
        }

        public List<ComboboxItemDto> GetCourseDurationsSelectListAsync()
        {
            var output = ((CourseDuration[])Enum.GetValues(typeof(CourseDuration))).Select(c => new ComboboxItemDto() { Value = ((int)c).ToString(), DisplayText = c.DisplayName() }).ToList();
            return output;
        }

        public List<ComboboxItemDto> GetResultStatusesSelectListAsync()
        {
            var output = ((ResultStatus[])Enum.GetValues(typeof(ResultStatus))).Select(c => new ComboboxItemDto() { Value = ((int)c).ToString(), DisplayText = c.DisplayName() }).ToList();
            return output;
        }

        public List<ComboboxItemDto> GetBloodGroupsSelectListAsync()
        {
            var output = ((BloodGroup[])Enum.GetValues(typeof(BloodGroup))).Select(c => new ComboboxItemDto() { Value = ((int)c).ToString(), DisplayText = c.DisplayName() }).ToList();
            return output;
        }

        public List<ComboboxItemDto> GetOfficeProgramsSelectListAsync()
        {
            var output = ((OfficePrograms[])Enum.GetValues(typeof(OfficePrograms))).Select(c => new ComboboxItemDto() { Value = ((int)c).ToString(), DisplayText = c.DisplayName() }).ToList();
            return output;
        }

        public async Task<int> GetNewRollAsync(bool bteb, int sessionId)
        {
            int? lastRoll = 0;
            if(bteb)
            {
                lastRoll = (await _studentRepository.GetAllAsync()).OrderByDescending(o=> o.ClassRoll).Where(x => x.Bteb && x.BtebSessionId == sessionId).FirstOrDefault()?.ClassRoll;
            }
            else
            {
                lastRoll = (await _studentRepository.GetAllAsync()).OrderByDescending(o => o.ClassRoll).Where(x => !x.Bteb).FirstOrDefault()?.ClassRoll;
            }
            return (lastRoll ?? 0) + 1;
        }
    }
}
