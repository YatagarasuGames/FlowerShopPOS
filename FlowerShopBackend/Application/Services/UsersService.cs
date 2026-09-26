using FlowerShop.Application.DTO;
using FlowerShop.Application.Interfaces;
using FlowerShop.Domain.Entities;
using FlowerShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Application.Services
{
    public class UsersService
    {
        private readonly FlowersDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly AuditService _auditService;

        public UsersService(FlowersDbContext context, IPasswordHasher passwordHasher, AuditService auditService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _auditService = auditService;
        }

        public async Task<IEnumerable<UserAdminDTO>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .OrderBy(u => u.Username)
                .Select(u => new UserAdminDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    RoleName = u.Role.Name,
                    RoleId = u.RoleId,
                    IsActive = u.IsActive
                })
                .ToListAsync();
        }

        public async Task<UserAdminDTO> CreateUserAsync(CreateStaffUserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                throw new InvalidOperationException("Логин и пароль обязательны.");

            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == dto.Username.ToLower().Trim()))
                throw new InvalidOperationException($"Пользователь '{dto.Username}' уже существует.");

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == dto.RoleName.ToLower())
                ?? throw new KeyNotFoundException($"Роль '{dto.RoleName}' не найдена.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username.Trim(),
                PasswordHash = _passwordHasher.Hash(dto.Password),
                RoleId = role.Id,
                IsActive = true
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                "CreateUser",
                "User",
                user.Id.ToString(),
                $"Создан сотрудник '{user.Username}' с ролью '{role.Name}'.");

            return new UserAdminDTO
            {
                Id = user.Id,
                Username = user.Username,
                RoleName = role.Name,
                RoleId = role.Id,
                IsActive = user.IsActive
            };
        }

        public async Task ToggleUserStatusAsync(Guid targetUserId, Guid currentAdminId)
        {
            var user = await _context.Users.FindAsync(targetUserId)
                ?? throw new KeyNotFoundException("Пользователь не найден.");

            if (user.Id == currentAdminId)
                throw new InvalidOperationException("Нельзя заблокировать собственную учетную запись.");

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            var action = user.IsActive ? "UnblockUser" : "BlockUser";
            var statusText = user.IsActive ? "разблокирован" : "заблокирован";

            await _auditService.LogAsync(
                action,
                "User",
                user.Id.ToString(),
                $"Сотрудник '{user.Username}' {statusText}.");
        }

        public async Task ResetPasswordAsync(Guid targetUserId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                throw new InvalidOperationException("Пароль должен содержать не менее 6 символов.");

            var user = await _context.Users.FindAsync(targetUserId)
                ?? throw new KeyNotFoundException("Пользователь не найден.");

            user.PasswordHash = _passwordHasher.Hash(newPassword);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                "ResetPassword",
                "User",
                user.Id.ToString(),
                $"Администратор сбросил пароль для сотрудника '{user.Username}'.");
        }
    }
}