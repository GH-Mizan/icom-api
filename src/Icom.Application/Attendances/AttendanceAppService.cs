using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Runtime.Session;
using Icom.Attendances.Dto;
using Icom.Entities;
using Icom.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Icom.Attendances
{
    public class AttendanceAppService: IcomAppServiceBase, IAttendanceAppService
    {
        private readonly IRepository<Attendance> _attendanceRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<BtebSession> _btebSessionRepository;
        private readonly IAbpSession _abpSession;
        public AttendanceAppService(
            IRepository<Attendance> attendanceRepository,
            IRepository<Student> studentRepository,
            IRepository<BtebSession> btebSessionRepository,
            IAbpSession abpSession
            )
        {
            _attendanceRepository = attendanceRepository;
            _studentRepository = studentRepository;
            _btebSessionRepository = btebSessionRepository;
            _abpSession = abpSession;
        }

        public async Task<PagedResultDto<AttendanceOutputDto>> GetPaginatedAttendancesAsync(AttendanceFilterDto filter)
        {
            var offDayAttendance = filter.Date.HasValue ? (await _attendanceRepository.GetAllAsync()).Where(x => x.Date.Date == filter.Date.Value.Date && x.OffDay).FirstOrDefault() : null;
            if (offDayAttendance != null)
            {
                var offDay = new AttendanceOutputDto()
                {
                    Id = offDayAttendance.Id,
                    Date = offDayAttendance.Date,
                    Remarks = offDayAttendance.Remarks,
                };
                return new PagedResultDto<AttendanceOutputDto>(1, new List<AttendanceOutputDto>() { offDay });
            }
            var searchText = string.IsNullOrEmpty(filter.SearchText) ? null : filter.SearchText.ToLower().Trim();

            var query = (from a in await _attendanceRepository.GetAllAsync()
                         join s in await _studentRepository.GetAllAsync() on a.StudentId equals s.Id
                         where (filter.StudentId == null || a.StudentId == filter.StudentId)
                         && (filter.Date == null || a.Date.Date == filter.Date.Value.Date)
                         && (filter.ActiveOnly == false || s.IsActive == true)
                         && (filter.Bteb == null || s.Bteb == filter.Bteb)
                         && (filter.Present == null || a.Present == filter.Present)
                         select new AttendanceOutputDto()
                         {
                             Id = a.Id,
                             Date = a.Date,
                             StudentId = a.StudentId,
                             StudentName = s.Name,
                             BTEB = s.Bteb,
                             Day = a.Day,
                             Present = a.Present,
                             Remarks = a.Remarks,
                         }).AsQueryable();

            if (searchText != null)
            {
                query = query.Where(x =>
                x.StudentName.ToLower().Trim().Contains(searchText));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Date)
                .Skip(filter.Skip)
                .Take(filter.Take).ToListAsync();

            return new PagedResultDto<AttendanceOutputDto>(totalCount, items);
        }

        public async Task<AttendanceEntryOutputDto> GetAttendancesAsync(DateTime? date)
        {
            var output = new AttendanceEntryOutputDto()
            {
                Attendances = new List<AttendanceOutputDto>()
            };
            var attendances = new List<AttendanceOutputDto>();

            if (!date.HasValue)
                date = (await _attendanceRepository.GetAllAsync()).OrderByDescending(o => o.Date).FirstOrDefault()?.Date.AddDays(1) ?? DateTime.UtcNow;

            
            var prevAttendances = await _attendanceRepository.GetAllListAsync(x => x.Date.Date == date.Value.Date);
            var students = await _studentRepository.GetAllListAsync(x => x.IsActive && !x.CourseCompleted && x.AdmisionDate.Date <= date.Value.Date && x.Course == IccCourses.ComputerOfficeApplication);
            students = students.OrderByDescending(s => s.Bteb).ThenBy(t => t.Id).ToList();
            var btebSessions = await _btebSessionRepository.GetAllListAsync(x => !x.IsExaminationHeld);
            //var offDaysList = (await _attendanceRepository.GetAllAsync()).Where(x => x.Date.Date < date.Value.Date).GroupBy(g => g.StudentId).Select(s => new
            //{
            //    StudentId = s.Key,
            //    TotalOffDays = s.Count(c => c.OffDay)
            //});
            if (prevAttendances.Count > 0)
            {
                output.EditMode = true;
            }

            var theLastDay = date.Value.Date.AddDays(-1);
            var theLastAttendaysDay = (await _attendanceRepository.GetAllAsync()).Where(x => !x.OffDay && x.Date.Date <= theLastDay).OrderByDescending(o => o.Date.Date).FirstOrDefault().Date.Date;
            var prevDayAttendances = await _attendanceRepository.GetAllListAsync(x => x.Date.Date == theLastAttendaysDay);

            foreach (var st in students)
            {
                var attendance = new AttendanceOutputDto();
                if (output.EditMode)
                {
                    var existingAttendance = prevAttendances.FirstOrDefault(f => f.StudentId == st.Id);
                    if (existingAttendance != null)
                    {
                        attendance.Day = existingAttendance.Day;
                        attendance.Present = existingAttendance.Present;
                        attendance.Remarks = existingAttendance.Remarks;
                    }
                    else
                    {
                        attendance.Day = 1;
                        //attendance.Present = existingAttendance.Present;
                        //attendance.Remarks = existingAttendance.Remarks;
                    }
                }
                else
                {
                    //attendance.Day = offDaysList.FirstOrDefault(f => f.StudentId == st.Id)?.TotalOffDays + 1 ?? 1; 
                    var totalDays = prevDayAttendances.FirstOrDefault(f=> f.StudentId == st.Id)?.Day;
                    attendance.Day = totalDays.HasValue ? totalDays.Value + 1 : 1;
                    //attendance.Present = existingAttendance.Present;
                    //attendance.Remarks = existingAttendance.Remarks;
                }

                attendance.Date = date.Value;
                attendance.StudentId = st.Id;
                attendance.StudentName = st.Name;
                attendance.BTEB = st.Bteb;
                attendance.IdentityNumber = st.IdentityNumber;
                attendance.BtebSession = attendance.BTEB ? btebSessions.First(f => f.Id == st.BtebSessionId).SessionName : null;
                output.Attendances.Add(attendance);
            }
            output.Date = date.Value;
            return output;
        }

        [UnitOfWork]
        public async Task CreateUpdateAttendance(AttendanceEntryInputDto input)
        {
            if(!input.EditMode)
            {
                await InsertAttendances(input.Attedances, input.Date, input.Course);
            }
            else
            {
                await _attendanceRepository.BatchDeleteAsync(x => x.Date.Date == input.Date.Date);
                await InsertAttendances(input.Attedances, input.Date, input.Course);
            }
        }

        public async Task DeclareOffDay(DateTime date, string reason)
        {
            var entity = new Attendance()
            {
                Date = date,
                Remarks = reason,
                OffDay = true
            };
            await _attendanceRepository.InsertAsync(entity);
        }

        public async Task AttendanceRemoveAsync(DateTime date)
        {
           await _attendanceRepository.BatchDeleteAsync(x => x.Date.Date == date);
        }

        private async Task InsertAttendances(List<AttendanceEntryDto> input, DateTime date, IccCourses course)
        {
            foreach (var a in input)
            {
                var entity = new Attendance()
                {
                    Date = date,
                    StudentId = a.StudentId,
                    Day = a.Day,
                    Present = a.Present,
                    EntryTime = a.EntryTime,
                    EndTime = a.EndTime,
                    Remarks = a.Remarks,
                    Course = IccCourses.ComputerOfficeApplication,
                    OffDay = a.OffDay,
                    TenantId = _abpSession.TenantId.Value
                };
                await _attendanceRepository.InsertAsync(entity);
            }
        }

    }
}
