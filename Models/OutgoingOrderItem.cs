namespace StorageManagement.API.Models
{
    public class OutgoingOrderItem
    {
        public int OutgoingOrderId { get; set; }
        public int? StorageItemId { get; set; }
        public int Quantity { get; set; }
        public string? CustomItemDescription { get; set; }
    }
}