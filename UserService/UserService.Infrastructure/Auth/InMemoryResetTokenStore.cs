using System.Collections.Concurrent;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure.Auth;

public class InMemoryResetTokenStore : IResetTokenStore
{
    private readonly ConcurrentDictionary<string, Guid> _tokens = new();

    public void Store(Guid userId, string token)
    {
        _tokens[token] = userId;
    }

    public Guid? Take(string token)
    {
        if (_tokens.TryRemove(token, out var userId))
            return userId;
        return null;
    }
}
