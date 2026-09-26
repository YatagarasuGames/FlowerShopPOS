namespace FlowerShop.Application.DTO
{
    public class UpdatePriceDTO
    {
        public float NewPrice { get; set; }
        public Guid ChangedByUserId { get; set; }
    }

    public class UpdateBatchPriceDTO
    {
        public float NewSellingPrice { get; set; }
    }
}
