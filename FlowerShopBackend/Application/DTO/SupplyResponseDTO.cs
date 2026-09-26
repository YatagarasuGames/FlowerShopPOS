namespace FlowerShop.Application.DTO
{
    public class SupplyResponseDTO
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Guid ReceivedByUserId { get; set; }
        public int Quantity { get; set; }
        public float PurchasePrice { get; set; }
        public float SellingPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
