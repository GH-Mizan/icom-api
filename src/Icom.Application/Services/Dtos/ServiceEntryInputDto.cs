using Icom.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icom.Services.Dtos
{
    public class ServiceEntryInputDto
    {
        public ServiceEntryDto Service { get; set; }
        public ServiceDueReceivedHistoryDto DueReceived { get; set; }
    }

    public class ServiceEntryDto
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; }
        public string ServiceTypes { get; set; } //Json value (service types array)
        public decimal ServiceCharge { get; set; }
        public decimal Discount { get; set; }
        public decimal NetServiceCharge { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Due { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public int ClientId { get; set; }
        public string Remarks { get; set; }
        public int TenantId { get; set; }
    }
}
