using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Infrastructure.Persistence;
using Xunit;

namespace UserService.IntegrationTests;

public class UserRepositoryIntegrationTests
{
    private readonly UserDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new UserDbContext(options);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ValidUser_ShouldSaveUserToDatabase()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword123",
            IsEmailVerified = false,
            Role = UserRole.User
        };

        // Act
        await _repository.AddAsync(user);

        // Assert
        var savedUser = await _context.Users.FindAsync(user.Id);
        Assert.NotNull(savedUser);
        Assert.Equal(user.Id, savedUser.Id);
        Assert.Equal(user.Username, savedUser.Username);
        Assert.Equal(user.Email, savedUser.Email);
        Assert.Equal(user.PasswordHash, savedUser.PasswordHash);
        Assert.Equal(user.IsEmailVerified, savedUser.IsEmailVerified);
        Assert.Equal(user.Role, savedUser.Role);
    }

    [Fact]
    public async Task AddAsync_UserWithAdminRole_ShouldSaveRoleCorrectly()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "adminuser",
            Email = "admin@example.com",
            PasswordHash = "hashedpassword123",
            IsEmailVerified = true,
            Role = UserRole.Admin
        };

        // Act
        await _repository.AddAsync(user);

        // Assert
        var savedUser = await _context.Users.FindAsync(user.Id);
        Assert.NotNull(savedUser);
        Assert.Equal(UserRole.Admin, savedUser.Role);
    }

    [Fact]
    public async Task GetByEmailAsync_ExistingEmail_ShouldReturnUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword123",
            IsEmailVerified = false,
            Role = UserRole.User
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Username, result.Username);
    }

    [Fact]
    public async Task GetByEmailAsync_NonExistingEmail_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_CaseInsensitiveSearch_ShouldReturnUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "Test@Example.Com",
            PasswordHash = "hashedpassword123",
            IsEmailVerified = false,
            Role = UserRole.User
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("test@example.com");

        // Assert - This test verifies if email search is case-insensitive
        // The result will depend on your database collation settings
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ShouldReturnUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword123",
            IsEmailVerified = false,
            Role = UserRole.User
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Username, result.Username);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WithUsers_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new() {
                Id = Guid.NewGuid(),
                Username = "user1",
                Email = "user1@example.com",
                PasswordHash = "hash1",
                IsEmailVerified = false,
                Role = UserRole.User
            },
            new() {
                Id = Guid.NewGuid(),
                Username = "user2",
                Email = "user2@example.com",
                PasswordHash = "hash2",
                IsEmailVerified = true,
                Role = UserRole.Admin
            },
            new() {
                Id = Guid.NewGuid(),
                Username = "user3",
                Email = "user3@example.com",
                PasswordHash = "hash3",
                IsEmailVerified = false,
                Role = UserRole.User
            }
        };

        foreach (var user in users)
        {
            _context.Users.Add(user);
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        foreach (var expectedUser in users)
        {
            var actualUser = result.FirstOrDefault(u => u.Id == expectedUser.Id);
            Assert.NotNull(actualUser);
            Assert.Equal(expectedUser.Username, actualUser.Username);
            Assert.Equal(expectedUser.Email, actualUser.Email);
        }
    }

    [Fact]
    public async Task UpdateAsync_ExistingUser_ShouldUpdateUserProperties()
    {
        // Arrange - Add initial user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "originaluser",
            Email = "original@example.com",
            PasswordHash = "originalhash",
            IsEmailVerified = false,
            Role = UserRole.User
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Modify user properties
        user.Username = "updateduser";
        user.Email = "updated@example.com";
        user.IsEmailVerified = true;
        user.Role = UserRole.Admin;

        // Act
        await _repository.UpdateAsync(user);

        // Assert
        var updatedUser = await _context.Users.FindAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("updateduser", updatedUser.Username);
        Assert.Equal("updated@example.com", updatedUser.Email);
        Assert.True(updatedUser.IsEmailVerified);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingUser_ShouldThrowException()
    {
        // Arrange
        var nonExistingUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "nonexisting",
            Email = "nonexisting@example.com",
            PasswordHash = "hash",
            IsEmailVerified = false
        };

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            () => _repository.UpdateAsync(nonExistingUser));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task GetByEmailAsync_InvalidEmail_ShouldReturnNull(string email)
    {
        // Act
        var result = await _repository.GetByEmailAsync(email);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Repository_ConcurrentOperations_ShouldHandleCorrectly()
    {
        // Arrange
        var users = Enumerable.Range(1, 10).Select(i => new User
        {
            Id = Guid.NewGuid(),
            Username = $"user{i}",
            Email = $"user{i}@example.com",
            PasswordHash = $"hash{i}",
            IsEmailVerified = i % 2 == 0
        }).ToList();

        // Act - Add users concurrently
        var tasks = users.Select(user => _repository.AddAsync(user)).ToArray();
        await Task.WhenAll(tasks);

        // Assert
        var allUsers = await _repository.GetAllAsync();
        Assert.Equal(10, allUsers.Count);

        foreach (var expectedUser in users)
        {
            var actualUser = allUsers.FirstOrDefault(u => u.Id == expectedUser.Id);
            Assert.NotNull(actualUser);
            Assert.Equal(expectedUser.Username, actualUser.Username);
        }
    }
}