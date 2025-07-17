namespace StorageManagement.API.Models
{
    public class SuperitemDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public List<SubItemQuantityDto> SubItems { get; set; } = new List<SubItemQuantityDto>();
    }

    public class SubItemQuantityDto
    {
        public int StorageItemId { get; set; }
        public int Quantity { get; set; }
    }
}
