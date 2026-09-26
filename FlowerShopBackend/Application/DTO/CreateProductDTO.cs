namespace FlowerShop.Application.DTO
{
    public class CreateProductDTO
    {
        public string Name { get; set; } = string.Empty;
        public float PurchasePrice { get; set; } // Закупочная цена первой партии
        public float Price { get; set; }         // Розничная цена продажи первой партии
        public int CurrentQuantity { get; set; } // Начальный остаток
        public string? ImageUrl { get; set; }
    }

    public class UpdateProductNameDTO
    {
        public string Name { get; set; } = string.Empty;
    }
}
