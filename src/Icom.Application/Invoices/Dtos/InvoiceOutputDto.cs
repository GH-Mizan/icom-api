using Icom.Enums;
using System;

namespace Icom.Invoices.Dtos
{
    public class InvoiceOutputDto
    {
        public  int Id { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientIdentificationName { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public string InvoiceTypeText { get; set; }
        public decimal TotalBill { get; set; }
        public string Remarks { get; set; }
    }
}
