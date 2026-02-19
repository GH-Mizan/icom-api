using Icom.Enums;
using System;

namespace Icom.Sales.Dtos
{
    public class SaleOutputDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DueAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentStatusText { get; set; }
        public int TenantId { get; set; }
    }
}
