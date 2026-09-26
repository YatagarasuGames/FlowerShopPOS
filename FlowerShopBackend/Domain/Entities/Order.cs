namespace FlowerShop.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid CashierId { get; set; }
        public DateTime CreatedAt { get; set; }
        public float TotalAmount { get; set; }

        public User Cashier { get; set; }
    }
}
