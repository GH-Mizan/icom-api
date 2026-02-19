namespace Icom.Products.Dtos
{
    public class ProductEntryDto
    {
        public int? Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int? BrandId { get; set; }
        public int SupplierId { get; set; }
        public string Remarks { get; set; }
        public int TenantId { get; set; }
    }
}
