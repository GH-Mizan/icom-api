using Icom.Enums;
using System;
using System.Collections.Generic;

namespace Icom.Invoices.Dtos
{
    public class InvoiceEntryInputDto
    {
        public InvoiceEntryDto Invoice { get; set; }
        public List<InvoiceDetailsEntryDto> InvoiceDetails { get; set; }
    }

    public class InvoiceEntryDto
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; }
        public int ClientId { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public string Remarks { get; set; }
        public decimal TotalBill { get; set; }
        public int TenantId { get; set; }
    }

    public class InvoiceDetailsEntryDto
    {
        public int? Id { get; set; }
        public int InvoiceId { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string SerialNumber { get; set; }
        public string SealText { get; set; }
        public string Category { get; set; }
        public  string Brand { get; set; }
        public ServiceType ServiceType { get; set; }
        public string ServiceTypeText { get; set; }
        public string WarrantyPeriod { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid Uid { get; set; }
    }
}
