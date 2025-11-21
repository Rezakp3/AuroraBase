// Infrastructure/Persistence/Seeders/DefaultDataSeeder.cs

using Core.Entities;
using Core.Entities.Auth;
using Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeders;

public static class DefaultDataSeeder
{
    public static async Task SeedAsync(MyContext context)
    {
        // Seed Roles
        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new() { Name = "SuperAdmin", Title = "مدیر سیستم خفن" },
                new() { Name = "Admin", Title = "مدیر سیستم" },
                new() { Name = "User", Title = "کاربر عادی" }
            };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        // Seed Settings (تنظیمات حیاتی)
        if (!await context.Settings.AnyAsync())
        {
            var settings = new List<Setting>
            {
                // تنظیمات امنیتی
                new() { Group = "Security", Key = "SuperAdminIds", Value = "[1]", DataType = ESettingDataType.Json, Description = "لیست ID کاربران سوپر ادمین" },
                new() { Group = "Security", Key = "UserRoleId", Value = "3", DataType = ESettingDataType.Int, Description = "ID نقش پیش‌فرض کاربر عادی" },
                
                // تنظیمات تست
                new() { Group = "General", Key = "TestMode", Value = "true", DataType = ESettingDataType.Bool }
            };

            await context.Settings.AddRangeAsync(settings);
            await context.SaveChangesAsync();
        }

        // Seed Menus
        if (!await context.Menus.AnyAsync())
        {
            var menus = new List<Menu>
            {
                new() { Title = "داشبورد", Route = "/dashboard" },
                new() { Title = "کاربران", Route = "/users" },
                new() { Title = "تنظیمات", Route = "/settings" }
            };

            await context.Menus.AddRangeAsync(menus);
            await context.SaveChangesAsync();
        }

        // Seed Services (برای تست مجوزدهی پویا)
        if (!await context.Services.AnyAsync())
        {
            var services = new List<Service>
            {
                new() {  ServiceName = "Auth", ServiceIdentifier = "Auth.Test", Address = "/Auth/TestPermission" }
            };
            await context.Services.AddRangeAsync(services);
            await context.SaveChangesAsync();
        }

        // انتساب مجوز Auth.Test به نقش SuperAdmin
        if (!await context.RoleServices.AnyAsync())
        {
            await context.RoleServices.AddAsync(new Core.Entities.Auth.Relation.RoleService { RoleId = 1, ServiceId = 1 });
            await context.SaveChangesAsync();
        }
    }
}