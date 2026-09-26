namespace FlowerShop.Application.DTO
{
    public class WriteOffResponseDTO
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }

        // Закупочные показатели
        public float PurchasePriceAtWriteOff { get; set; }
        public float TotalPurchaseCost => (float)Math.Round(Quantity * PurchasePriceAtWriteOff, 2);

        // Розничные показатели
        public float RetailPriceAtWriteOff { get; set; }
        public float TotalRetailLoss => (float)Math.Round(Quantity * RetailPriceAtWriteOff, 2);

        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
