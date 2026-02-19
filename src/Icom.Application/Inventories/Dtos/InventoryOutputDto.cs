namespace Icom.Inventories.Dtos
{
    public class InventoryOutputDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SerialNo { get; set; }
        public int Quantity { get; set; }
        public int TenantId { get; set; }
    }
}
