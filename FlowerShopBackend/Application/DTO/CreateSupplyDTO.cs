namespace FlowerShop.Application.DTO
{
    public class CreateSupplyDTO
    {
        public Guid ProductId { get; set; }
        public Guid ReceivedByUserId { get; set; }
        public int Quantity { get; set; }
        public float PurchasePrice { get; set; }
        public float SellingPrice { get; set; }
    }
}
