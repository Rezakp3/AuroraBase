using Core.Entities.Auth;
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
                new() { Id = 1, Name = "SuperAdmin", Title = "مدیر سیستم خفن" },
                new() { Id = 2, Name = "Admin", Title = "مدیر سیستم" },
                new() { Id = 3, Name = "User", Title = "کاربر عادی" }
            };
            
            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        // Seed Menus
        if (!await context.Menus.AnyAsync())
        {
            var menus = new List<Menu>
            {
                new() { Id = 1, Title = "داشبورد", Route = "/dashboard" },
                new() { Id = 2, Title = "کاربران", Route = "/users" }
            };
            
            await context.Menus.AddRangeAsync(menus);
            await context.SaveChangesAsync();
        }
    }
}