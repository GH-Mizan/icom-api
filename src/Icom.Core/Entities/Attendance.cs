using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Icom.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icom.Entities
{
    public class Attendance : FullAuditedEntity, IMustHaveTenant
    {
        public DateTime Date { get; set; }
        public int StudentId { get; set; }
        public int Day { get; set; }
        public bool Present { get; set; }
        public string EntryTime { get; set; }
        public string EndTime { get; set; }
        public string Remarks { get; set; }
        public bool OffDay { get; set; }
        public IccCourses Course { get; set; }
        public int TenantId { get; set; }
    }
}
