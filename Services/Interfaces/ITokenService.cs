using taskmanager_api.Entities;

namespace taskmanager_api.Services.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAtUtc) CreateToken(User user);
}