namespace StorageManagement.API.Models
{
    public class OutgoingOrder
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Recipient { get; set; }
        public string SerialNumber { get; set; }
        public string Status { get; set; }
    }
}