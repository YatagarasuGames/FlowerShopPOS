using System.Net;
using System.Security.Claims;
using FlowerShop.Domain.Entities;
using FlowerShop.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Application.Services
{
    public class AuditService
    {
        private readonly FlowersDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(FlowersDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string action, string entityName, string? entityId, string details)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            Guid? userId = null;
            var userIdClaim = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var parsedId))
            {
                userId = parsedId;
            }

            var username = httpContext?.User?.Identity?.Name
                           ?? httpContext?.User?.FindFirst(ClaimTypes.Name)?.Value
                           ?? "Система";

            var ip = GetClientIpAddress(httpContext);

            await LogInternalAsync(userId, username, action, entityName, entityId, details, ip);
        }

        public async Task LogAsync(Guid? userId, string username, string action, string entityName, string? entityId, string details, string? ip = null)
        {
            var resolvedIp = ip ?? GetClientIpAddress(_httpContextAccessor.HttpContext);
            await LogInternalAsync(userId, username, action, entityName, entityId, details, resolvedIp);
        }

        private static string GetClientIpAddress(HttpContext? context)
        {
            if (context == null) return "127.0.0.1";

            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor) && !string.IsNullOrWhiteSpace(forwardedFor))
            {
                var clientIp = forwardedFor.ToString().Split(',')[0].Trim();
                if (!string.IsNullOrEmpty(clientIp)) return clientIp;
            }

            if (context.Request.Headers.TryGetValue("X-Real-IP", out var realIp) && !string.IsNullOrWhiteSpace(realIp))
            {
                return realIp.ToString().Trim();
            }

            var remoteIp = context.Connection.RemoteIpAddress;
            if (remoteIp == null) return "127.0.0.1";

            if (IPAddress.IsLoopback(remoteIp) || remoteIp.ToString() == "::1")
            {
                return "127.0.0.1";
            }

            if (remoteIp.IsIPv4MappedToIPv6)
            {
                return remoteIp.MapToIPv4().ToString();
            }

            return remoteIp.ToString();
        }

        private async Task LogInternalAsync(Guid? userId, string username, string action, string entityName, string? entityId, string details, string? ip)
        {
            var log = new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Username = string.IsNullOrWhiteSpace(username) ? "Система" : username,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Details = details,
                IpAddress = ip,
                CreatedAt = DateTime.UtcNow
            };

            await _context.AuditLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetLogsAsync(int limit = 100)
        {
            return await _context.AuditLogs
                .OrderByDescending(a => a.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }
    }
}