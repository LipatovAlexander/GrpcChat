using System.Collections.Concurrent;
using Server.Models;

namespace Server.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task RemoveAsync(User user, CancellationToken cancellationToken);
    Task UpdateAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<List<User>> GetAllOnlineAsync(CancellationToken cancellationToken);
}

public sealed class UserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<string, User> _concurrentDictionary = new();

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _concurrentDictionary.TryAdd(user.Id, user);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(User user, CancellationToken cancellationToken)
    {
        _concurrentDictionary.Remove(user.Id, out _);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        _concurrentDictionary.Remove(user.Id, out _);
        _concurrentDictionary.TryAdd(user.Id, user);
        return Task.CompletedTask;
    }

    public Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var result = _concurrentDictionary.TryGetValue(id, out var user) ? user : null;
        return Task.FromResult(result);
    }

    public Task<List<User>> GetAllOnlineAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_concurrentDictionary.Values.Where(u => u.IsOnline).ToList());
    }
}