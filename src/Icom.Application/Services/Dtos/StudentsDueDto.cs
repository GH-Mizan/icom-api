using System;

namespace Icom.Services.Dtos
{
    public class StudentsDueDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FathersName { get; set; }
        public string ContactNumber { get; set; }
        public DateTime AdmissionDate { get; set; }
        public string SessionId { get; set; }
        public string SessionText { get; set; }
        public decimal CourseFee { get; set; }
        public decimal AdditionalCharge { get; set; }
        public decimal TotalFees { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDue { get; set; }
    }
}
