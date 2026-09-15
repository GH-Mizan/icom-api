using System;

namespace Icom.Services.Dtos
{
    public class StudentsDueInputDto
    {
        public int? SessionId { get; set; }
        public int? StudentId { get; set; }
        public int CourseId { get; set; }
        public decimal AdditionalCharge { get; set; }
        public DateTime? AdmissionBefore { get; set; }
    }
}
