namespace StorageManagement.API.Models
{
    public class Superitem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public List<SuperitemSubItemQuantity> SubItems { get; set; } = new List<SuperitemSubItemQuantity>();
    }

    public class SuperitemStorageItem
    {
        public int SuperitemId { get; set; }
        public int StorageItemId { get; set; }
    }
}
