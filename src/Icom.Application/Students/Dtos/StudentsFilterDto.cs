using Icom.Common;

namespace Icom.Students.Dtos
{
    public class StudentsFilterDto : FilterBaseDto
    {
        public bool? IsBteb { get; set; }
        public bool? IsBtebAdmitted { get; set; }
        public bool? IsBtebRegistered { get; set; }
        public bool? CourseCompleted { get; set; }
        public bool? CertificateDistributed { get; set; }
        public int? BtebSessionId { get; set; }
        public int? CourseId { get; set; }
        public bool? IsActive { get; set; }
    }
}
