using System.Collections.Concurrent;
using Server.Models;

namespace Server.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task RemoveAsync(User user, CancellationToken cancellationToken);
    Task<ICollection<User>> GetAllAsync(CancellationToken cancellationToken);
}

public sealed class UserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<User, byte> _concurrentDictionary = new();

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _concurrentDictionary.TryAdd(user, 0);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(User user, CancellationToken cancellationToken)
    {
        _concurrentDictionary.Remove(user, out _);
        return Task.CompletedTask;
    }

    public Task<ICollection<User>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_concurrentDictionary.Keys);
    }
}