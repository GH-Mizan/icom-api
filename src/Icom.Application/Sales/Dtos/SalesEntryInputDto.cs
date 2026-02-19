using Icom.Enums;
using System;
using System.Collections.Generic;

namespace Icom.Sales.Dtos
{
    public class SalesEntryInputDto
    {
        public SalesEntryDto Sales { get; set; }
        public List<SalesDetailsEntryDto> SalesDetails { get; set; }
        public DueReceivedHistoryDto DueReceived { get; set; }
    }

    public class SalesEntryDto
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; }
        public int ClientId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DueAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string SalesBy { get; set; }
        public int TenantId { get; set; }
    }

    public class SalesDetailsEntryDto
    {
        public int? Id { get; set; }
        public int SalesId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SerialNo { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remarks { get; set; }
    }
}
