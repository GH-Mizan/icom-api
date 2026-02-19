namespace Icom.Suppliers.Dtos
{
    public class SupplierOutputDto
    {
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public string ShortName { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public int TenantId { get; set; }
    }
}
