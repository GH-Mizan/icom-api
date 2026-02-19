using System;
using System.Collections.Generic;

namespace Icom.Sales.Dtos
{
    public class SaleEntryDto
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        public int ClientId { get; set; }
        public bool HasDue { get; set; }
        public decimal OverallDiscount { get; set; }
        public int TenantId { get; set; }
        public string Reference { get; set; }
        public string ProductsJson { get; set; }
    }

    public class SaleProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SerialNo { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal NetPrice { get; set; }
    }
}
