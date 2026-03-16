using Icom.Enums;
using System;

namespace Icom.ServiceExpenses.Dtos
{
    public class ServiceExpenseEntryDto
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        public ServiceExpenseType Type { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public int TenantId { get; set; }
    }
}
