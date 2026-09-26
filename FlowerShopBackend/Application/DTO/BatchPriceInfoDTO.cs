namespace FlowerShop.Application.DTO
{
    public class BatchPriceInfoDTO
    {
        public Guid BatchId { get; set; }
        public float PurchasePrice { get; set; }
        public float SellingPrice { get; set; }
        public int RemainingQuantity { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}
