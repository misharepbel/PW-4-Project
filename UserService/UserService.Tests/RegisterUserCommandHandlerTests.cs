using Microsoft.AspNetCore.Identity;
using Moq;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Application.Users.Commands;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.UnitTests;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;
    private readonly Mock<IUserRegisteredProducer> _mockProducer;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
        _mockProducer = new Mock<IUserRegisteredProducer>();

        _handler = new RegisterUserCommandHandler(
            _mockUserRepository.Object,
            _mockPasswordHasher.Object,
            _mockProducer.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateUserAndReturnId()
    {
        // Arrange
        var command = new RegisterUserCommand("testuser", "test@example.com", "password123");
        var hashedPassword = "hashedPassword123";
        var cancellationToken = CancellationToken.None;

        _mockPasswordHasher
            .Setup(x => x.HashPassword(It.IsAny<User>(), command.Password))
            .Returns(hashedPassword);

        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mockProducer
            .Setup(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateUserWithCorrectProperties()
    {
        // Arrange
        var command = new RegisterUserCommand("testuser", "test@example.com", "password123");
        var hashedPassword = "hashedPassword123";
        var cancellationToken = CancellationToken.None;
        User capturedUser = null;

        _mockPasswordHasher
            .Setup(x => x.HashPassword(It.IsAny<User>(), command.Password))
            .Returns(hashedPassword);

        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Callback<User>(user => capturedUser = user)
            .Returns(Task.CompletedTask);

        _mockProducer
            .Setup(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(capturedUser);
        Assert.NotEqual(Guid.Empty, capturedUser.Id);
        Assert.Equal(command.Username, capturedUser.Username);
        Assert.Equal(command.Email, capturedUser.Email);
        Assert.Equal(hashedPassword, capturedUser.PasswordHash);
        Assert.False(capturedUser.IsEmailVerified);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldHashPasswordCorrectly()
    {
        // Arrange
        var command = new RegisterUserCommand("testuser", "test@example.com", "password123");
        var hashedPassword = "hashedPassword123";
        var cancellationToken = CancellationToken.None;

        _mockPasswordHasher
            .Setup(x => x.HashPassword(It.IsAny<User>(), command.Password))
            .Returns(hashedPassword);

        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mockProducer
            .Setup(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _mockPasswordHasher.Verify(
            x => x.HashPassword(It.Is<User>(u => u.Username == command.Username), command.Password),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCallRepositoryAddAsync()
    {
        // Arrange
        var command = new RegisterUserCommand("testuser", "test@example.com", "password123");
        var hashedPassword = "hashedPassword123";
        var cancellationToken = CancellationToken.None;

        _mockPasswordHasher
            .Setup(x => x.HashPassword(It.IsAny<User>(), command.Password))
            .Returns(hashedPassword);

        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mockProducer
            .Setup(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _mockUserRepository.Verify(
            x => x.AddAsync(It.Is<User>(u =>
                u.Username == command.Username &&
                u.Email == command.Email &&
                u.PasswordHash == hashedPassword &&
                u.IsEmailVerified == false)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldPublishUserRegisteredEvent()
    {
        // Arrange
        var command = new RegisterUserCommand("testuser", "test@example.com", "password123");
        var hashedPassword = "hashedPassword123";
        var cancellationToken = CancellationToken.None;

        _mockPasswordHasher
            .Setup(x => x.HashPassword(It.IsAny<User>(), command.Password))
            .Returns(hashedPassword);

        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mockProducer
            .Setup(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _mockProducer.Verify(
            x => x.PublishAsync(It.Is<UserRegisteredEvent>(e =>
                e.Email == command.Email &&
                e.Username == command.Username &&
                e.UserId != Guid.Empty),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new RegisterUserCommand("testuser", "test@example.com", "password123");
        var hashedPassword = "hashedPassword123";
        var cancellationToken = CancellationToken.None;
        var expectedException = new InvalidOperationException("Database error");

        _mockPasswordHasher
            .Setup(x => x.HashPassword(It.IsAny<User>(), command.Password))
            .Returns(hashedPassword);

        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        Assert.Equal(expectedException, exception);
    }

    [Fact]
    public async Task Handle_ProducerThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new RegisterUserCommand("testuser", "test@example.com", "password123");
        var hashedPassword = "hashedPassword123";
        var cancellationToken = CancellationToken.None;
        var expectedException = new InvalidOperationException("Message queue error");

        _mockPasswordHasher
            .Setup(x => x.HashPassword(It.IsAny<User>(), command.Password))
            .Returns(hashedPassword);

        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mockProducer
            .Setup(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>(), cancellationToken))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        Assert.Equal(expectedException, exception);
    }

    [Fact]
    public async Task Handle_PasswordHasherThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new RegisterUserCommand("testuser", "test@example.com", "password123");
        var cancellationToken = CancellationToken.None;
        var expectedException = new ArgumentException("Invalid password");

        _mockPasswordHasher
            .Setup(x => x.HashPassword(It.IsAny<User>(), command.Password))
            .Throws(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.Handle(command, cancellationToken));

        Assert.Equal(expectedException, exception);
    }

    [Theory]
    [InlineData("", "test@example.com", "password123")]
    [InlineData("testuser", "", "password123")]
    [InlineData("testuser", "test@example.com", "")]
    public async Task Handle_WithEmptyStringValues_ShouldStillCreateUser(string username, string email, string password)
    {
        // Arrange
        var command = new RegisterUserCommand(username, email, password);
        var hashedPassword = "hashedPassword123";
        var cancellationToken = CancellationToken.None;

        _mockPasswordHasher
            .Setup(x => x.HashPassword(It.IsAny<User>(), command.Password))
            .Returns(hashedPassword);

        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mockProducer
            .Setup(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task Handle_CancellationRequested_ShouldRespectCancellationToken()
    {
        // Arrange
        var command = new RegisterUserCommand("testuser", "test@example.com", "password123");
        var hashedPassword = "hashedPassword123";
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        var cancellationToken = cancellationTokenSource.Token;

        _mockPasswordHasher
            .Setup(x => x.HashPassword(It.IsAny<User>(), command.Password))
            .Returns(hashedPassword);

        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mockProducer
            .Setup(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>(), cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldGenerateUniqueUserIds()
    {
        // Arrange
        var command1 = new RegisterUserCommand("testuser1", "test1@example.com", "password123");
        var command2 = new RegisterUserCommand("testuser2", "test2@example.com", "password456");
        var hashedPassword = "hashedPassword123";
        var cancellationToken = CancellationToken.None;

        _mockPasswordHasher
            .Setup(x => x.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
            .Returns(hashedPassword);

        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mockProducer
            .Setup(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result1 = await _handler.Handle(command1, cancellationToken);
        var result2 = await _handler.Handle(command2, cancellationToken);

        // Assert
        Assert.NotEqual(result1, result2);
        Assert.NotEqual(Guid.Empty, result1);
        Assert.NotEqual(Guid.Empty, result2);
    }
}