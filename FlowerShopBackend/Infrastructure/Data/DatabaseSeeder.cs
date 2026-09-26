using FlowerShop.Application.Interfaces;
using FlowerShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(FlowersDbContext context, IPasswordHasher hasher)
        {
            // 1. Создание всех системных прав (Permissions)
            var permissions = new List<Permission>
            {
                new() { Id = Guid.NewGuid(), Name = "products.view", Description = "Просмотр каталога товаров" },
                new() { Id = Guid.NewGuid(), Name = "products.manage", Description = "Создание, удаление товаров и редактирование цен партий" },
                new() { Id = Guid.NewGuid(), Name = "orders.create", Description = "Оформление чеков на кассе" },
                new() { Id = Guid.NewGuid(), Name = "orders.view", Description = "Просмотр журнала заказов и продаж" },
                new() { Id = Guid.NewGuid(), Name = "inventory.writeoff", Description = "Фиксация и просмотр списаний товара" },
                new() { Id = Guid.NewGuid(), Name = "inventory.supply", Description = "Приемка поставок и просмотр журнала поступлений" },
                new() { Id = Guid.NewGuid(), Name = "analytics.view", Description = "Просмотр отчетов по выручке, прибыли и оценке склада" },
                new() { Id = Guid.NewGuid(), Name = "users.manage", Description = "Управление сотрудниками: создание, блокировка, сброс паролей" },
                new() { Id = Guid.NewGuid(), Name = "audit.view", Description = "Просмотр журнала безопасности и логов аудита" }
            };

            foreach (var perm in permissions)
            {
                if (!await context.Permissions.AnyAsync(p => p.Name == perm.Name))
                {
                    await context.Permissions.AddAsync(perm);
                }
            }
            await context.SaveChangesAsync();

            var allPermissions = await context.Permissions.ToListAsync();

            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole == null)
            {
                adminRole = new Role { Id = Guid.NewGuid(), Name = "Admin" };
                await context.Roles.AddAsync(adminRole);
            }

            var ownerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Owner");
            if (ownerRole == null)
            {
                ownerRole = new Role { Id = Guid.NewGuid(), Name = "Owner" };
                await context.Roles.AddAsync(ownerRole);
            }

            var cashierRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Cashier");
            if (cashierRole == null)
            {
                cashierRole = new Role { Id = Guid.NewGuid(), Name = "Cashier" };
                await context.Roles.AddAsync(cashierRole);
            }
            await context.SaveChangesAsync();


            foreach (var perm in allPermissions)
            {
                await EnsureRolePermissionAsync(context, adminRole.Id, perm.Id);
            }

            var ownerExcludedPerms = new[] { "users.manage", "audit.view" };
            var ownerAllowedPerms = allPermissions.Where(p => !ownerExcludedPerms.Contains(p.Name));
            foreach (var perm in ownerAllowedPerms)
            {
                await EnsureRolePermissionAsync(context, ownerRole.Id, perm.Id);
            }

            var cashierPermNames = new[]
            {
                "products.view",
                "orders.create",
                "orders.view",
                "inventory.supply",
                "inventory.writeoff"
            };
            var cashierAllowedPerms = allPermissions.Where(p => cashierPermNames.Contains(p.Name));
            foreach (var perm in cashierAllowedPerms)
            {
                await EnsureRolePermissionAsync(context, cashierRole.Id, perm.Id);
            }
            await context.SaveChangesAsync();

         
        }

        private static async Task EnsureRolePermissionAsync(FlowersDbContext context, Guid roleId, Guid permissionId)
        {
            var exists = await context.RolePermissions.AnyAsync(rp =>
                rp.RoleId == roleId && rp.PermissionId == permissionId);

            if (!exists)
            {
                await context.RolePermissions.AddAsync(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                });
            }
        }
    }
}