namespace FlowerShop.Application.DTO
{
    public class ProductResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Price { get; set; }
        public float MinPrice { get; set; }
        public float MaxPrice { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
        public List<BatchPriceInfoDTO> ActiveBatches { get; set; } = new();
    }
}
