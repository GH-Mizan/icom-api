using Icom.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icom.Services.Dtos
{
    public class ServiceOutputDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; }
        public string ServiceTypes { get; set; } //Json value (service types array)
        public string ServiceTypeNames { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Due { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentStatusText { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public ClientType ClientType { get; set; }
        public string Remarks { get; set; }
        public int TenantId { get; set; }
    }
}
