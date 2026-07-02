using System;

namespace Icom.Attendances.Dto
{
    public class AttendanceOutputDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string IdentityNumber { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public bool BTEB { get; set; }
        public string BtebSession { get; set; }
        public int Day { get; set; }
        public bool Present { get; set; }
        public string EntryTime { get; set; }
        public string EndTime { get; set; }
        public string Remarks { get; set; }
        public Guid Uid { get; set; }
        public int TenantId { get; set; }
    }
}
