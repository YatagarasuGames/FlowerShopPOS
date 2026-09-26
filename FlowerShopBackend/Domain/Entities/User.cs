namespace FlowerShop.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public Guid RoleId { get; set; }
        public bool IsActive { get; set; } = true;

        public Role Role { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
