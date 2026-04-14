using System;

namespace Icom.Sales.Dtos
{
    public class SalesDetailsOutputDto
    {
        public int? Id { get; set; }
        public int SalesId { get; set; }
        public DateTime Date { get; set; }
        public string ClientName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string SerialNo { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remarks { get; set; }
    }
}
