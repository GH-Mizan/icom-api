using System;
using System.Collections.Generic;

namespace Icom.Attendances.Dto
{
    public class AttendanceEntryOutputDto
    {
        public bool EditMode { get; set; }
        public DateTime Date { get; set; }
        public List<AttendanceOutputDto> Attendances { get; set; }
    }
}
