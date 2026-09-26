namespace FlowerShop.Application.DTO
{
    public class TopProductDTO
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public float TotalRevenueGenerated { get; set; }
    }
}
