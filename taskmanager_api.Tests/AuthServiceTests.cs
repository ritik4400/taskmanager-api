using Moq;
using taskmanager_api.DTOs;
using taskmanager_api.Entities;
using taskmanager_api.Exceptions;
using taskmanager_api.Repositories.Interfaces;
using taskmanager_api.Services;
using taskmanager_api.Services.Interfaces;
using Xunit;

namespace taskmanager_api.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<ITokenService> _tokens = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _tokens.Setup(t => t.CreateToken(It.IsAny<User>()))
               .Returns(("fake-token", DateTime.UtcNow.AddHours(1)));

        _service = new AuthService(_users.Object, _tokens.Object);
    }

    [Fact]
    public async Task RegisterAsync_NewEmail_LowercasesEmailAndHashesPassword()
    {
        User? saved = null;
        _users.Setup(u => u.EmailExistsAsync("ritik@test.com")).ReturnsAsync(false);
        _users.Setup(u => u.AddAsync(It.IsAny<User>()))
              .Callback<User>(u => saved = u)
              .Returns(Task.CompletedTask);

        var dto = new RegisterDto
        {
            FullName = "  Ritik  ",
            Email = "  Ritik@Test.com ",
            Password = "Test@1234"
        };

        var result = await _service.RegisterAsync(dto);

        Assert.NotNull(saved);
        Assert.Equal("ritik@test.com", saved!.Email);
        Assert.Equal("Ritik", saved.FullName);
        Assert.NotEqual("Test@1234", saved.PasswordHash);                              // never stored as plain text
        Assert.True(BCrypt.Net.BCrypt.Verify("Test@1234", saved.PasswordHash));        // but it verifies
        Assert.Equal("fake-token", result.Token);
    }

    [Fact]
    public async Task RegisterAsync_EmailAlreadyExists_ThrowsConflictAndDoesNotSave()
    {
        _users.Setup(u => u.EmailExistsAsync("ritik@test.com")).ReturnsAsync(true);

        var dto = new RegisterDto { FullName = "Ritik", Email = "ritik@test.com", Password = "Test@1234" };

        await Assert.ThrowsAsync<ConflictException>(() => _service.RegisterAsync(dto));

        _users.Verify(u => u.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_CorrectPassword_ReturnsToken()
    {
        var user = MakeUser("ritik@test.com", "Test@1234");
        _users.Setup(u => u.GetByEmailAsync("ritik@test.com")).ReturnsAsync(user);

        var result = await _service.LoginAsync(new LoginDto { Email = "Ritik@Test.com", Password = "Test@1234" });

        Assert.Equal("fake-token", result.Token);
        Assert.Equal("ritik@test.com", result.Email);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ThrowsUnauthorized()
    {
        var user = MakeUser("ritik@test.com", "Test@1234");
        _users.Setup(u => u.GetByEmailAsync("ritik@test.com")).ReturnsAsync(user);

        var ex = await Assert.ThrowsAsync<UnauthorizedException>(
            () => _service.LoginAsync(new LoginDto { Email = "ritik@test.com", Password = "WrongPass1" }));

        Assert.Equal("Invalid email or password.", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_UnknownEmail_ThrowsSameErrorAsWrongPassword()
    {
        _users.Setup(u => u.GetByEmailAsync("ghost@test.com")).ReturnsAsync((User?)null);

        var ex = await Assert.ThrowsAsync<UnauthorizedException>(
            () => _service.LoginAsync(new LoginDto { Email = "ghost@test.com", Password = "whatever1" }));

        // Same message as a wrong password, so attackers can't tell which emails exist
        Assert.Equal("Invalid email or password.", ex.Message);
    }

    private static User MakeUser(string email, string password) => new()
    {
        Id = 1,
        FullName = "Ritik",
        Email = email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
    };
}