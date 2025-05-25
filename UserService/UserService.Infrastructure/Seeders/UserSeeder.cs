using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Seed;

public class UserSeeder
{
    public static async Task SeedAsync(UserDbContext context, IPasswordHasher<User> hasher)
    {
        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync()) return;

        var users = new List<User>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@tea.com",
                Role = UserRole.Admin,
                IsEmailVerified = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "user1",
                Email = "user1@tea.com",
                Role = UserRole.User,
                IsEmailVerified = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "user2",
                Email = "user2@tea.com",
                Role = UserRole.User,
                IsEmailVerified = true
            }
        };

        users[0].PasswordHash = hasher.HashPassword(users[0], "admin123");
        users[1].PasswordHash = hasher.HashPassword(users[1], "user123");
        users[2].PasswordHash = hasher.HashPassword(users[2], "user123");

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }
}
