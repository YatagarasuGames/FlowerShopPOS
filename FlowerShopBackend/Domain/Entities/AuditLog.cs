namespace FlowerShop.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Username { get; set; } = "Система";
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string Details { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
