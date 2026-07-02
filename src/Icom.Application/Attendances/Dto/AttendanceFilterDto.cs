using Icom.Common;
using System;

namespace Icom.Attendances.Dto
{
    public class AttendanceFilterDto: FilterBaseDto
    {
        public DateTime? Date { get; set; }
        public int? StudentId { get; set; }
        public bool? Bteb { get; set; }
        public bool ActiveOnly { get; set; }
        public bool? Present { get; set; }
    }
}
