namespace FlowerShop.Application.DTO
{
    public class UserAdminDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public Guid RoleId { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateStaffUserDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RoleName { get; set; } = "Cashier";
    }

    public class ResetPasswordDTO
    {
        public string NewPassword { get; set; } = string.Empty;
    }
}