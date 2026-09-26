using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FlowerShop.Application.DTO;
using FlowerShop.Application.Interfaces;
using FlowerShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FlowerShop.Application.Services
{
    public class AuthService
    {
        private readonly FlowersDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly AuditService _auditService;

        public AuthService(
            IPasswordHasher passwordHasher,
            IConfiguration configuration,
            FlowersDbContext dbContext,
            AuditService auditService)
        {
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _dbContext = dbContext;
            _auditService = auditService;
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO dto)
        {
            var user = await _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == dto.Username.ToLower());

            if (user == null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
            {
                await _auditService.LogAsync(
                    user?.Id,
                    dto.Username,
                    "LoginFailed",
                    "Auth",
                    user?.Id.ToString(),
                    $"Неудачная попытка входа с логином '{dto.Username}'. Неверный логин или пароль.");

                throw new UnauthorizedAccessException("Неверный логин или пароль.");
            }

            if (!user.IsActive)
            {
                await _auditService.LogAsync(
                    user.Id,
                    user.Username,
                    "LoginBlocked",
                    "Auth",
                    user.Id.ToString(),
                    $"Попытка входа заблокированного сотрудника '{user.Username}'. Доступ отклонен.");

                throw new UnauthorizedAccessException("Ваша учетная запись заблокирована администратором.");
            }

            var permissions = await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == user.RoleId)
                .Select(rp => rp.Permission.Name)
                .ToListAsync();

            var token = GenerateJwtToken(user.Id, user.Username, user.Role.Name, permissions);

            await _auditService.LogAsync(
                user.Id,
                user.Username,
                "LoginSuccess",
                "Auth",
                user.Id.ToString(),
                $"Сотрудник '{user.Username}' успешно вошел в систему (Роль: {user.Role.Name}).");

            return new AuthResponseDTO
            {
                Token = token,
                Username = user.Username,
                Role = user.Role.Name,
                Permissions = permissions
            };
        }

        private string GenerateJwtToken(Guid userId, string username, string roleName, List<string> permissions)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, username),
                new(ClaimTypes.Role, roleName)
            };

            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpiryHours"] ?? "12")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}