using Icom.Common;
using Icom.Enums;
using System;

namespace Icom.Services.Dtos
{
    public class ServicesFilterDto : FilterBaseDto
    {
        public int? ClientId { get; set; }
        public bool DueOnly { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public bool DateRangeSearch { get; set; }
        public bool MonthlySearch { get; set; }
        public bool LifetimeDue { get; set; }
        public string ServiceType { get; set; }
    }
}
