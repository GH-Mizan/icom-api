using Icom.Enums;
using System;
using System.Collections.Generic;

namespace Icom.Attendances.Dto
{
    public class AttendanceEntryInputDto
    {
        public bool EditMode { get; set; }
        public DateTime Date { get; set; }
        public IccCourses Course { get; set; }
        public List<AttendanceEntryDto> Attedances { get; set; }
    }

    public class AttendanceEntryDto
    {
        public int StudentId { get; set; }
        public int Day { get; set; }
        public bool Present { get; set; }
        public string EntryTime { get; set; }
        public string EndTime { get; set; }
        public string Remarks { get; set; }
        public bool OffDay { get; set; }
    }
}
