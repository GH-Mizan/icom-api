using System;

namespace Icom.SetupExpenses.Dtos
{
    public class SetupExpenseEntryDto
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        public string Purpose { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public int TenantId { get; set; }
    }
}
