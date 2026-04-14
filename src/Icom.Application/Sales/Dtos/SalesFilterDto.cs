using Icom.Common;
using System;

namespace Icom.Sales.Dtos
{
    public class SalesFilterDto : FilterBaseDto
    {
        public bool IncludeDateSearch { get; set; }
        public int? ClientId { get; set; }
        public int? CategoryId { get; set; }
        public bool DueOnly { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public bool DateRangeSearch { get; set; }
        public bool MonthlySearch { get; set; }
        public bool LifetimeDue { get; set; }
    }
}
