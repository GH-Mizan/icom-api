using System;

namespace Icom.Pricelists.Dtos
{
    public class PricelistOutputDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SerialNo { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string WarrantyPeriod { get; set; }
        public int Quantity { get; set; }
        public decimal PurchaseUnitPrice { get; set; }
        public decimal? BodyUnitPrice { get; set; }
        public decimal? OnlineUnitPrice { get; set; }
        public decimal? SaleUnitPrice { get; set; }
        public int TenantId { get; set; }
    }
}
