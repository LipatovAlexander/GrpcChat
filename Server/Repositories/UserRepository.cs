using System.Collections.Concurrent;
using Server.Models;

namespace Server.Repositories;

public interface IUserRepository
{
    void Add(User user);
    User? GetById(string id);
    IEnumerable<User> GetAllOnline();
}

public sealed class UserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<string, User> _concurrentDictionary = new();

    public void Add(User user)
    {
        _concurrentDictionary.TryAdd(user.Id, user);
    }

    public User? GetById(string id)
    {
        return _concurrentDictionary.TryGetValue(id, out var user) ? user : null;
    }

    public IEnumerable<User> GetAllOnline()
    {
        return _concurrentDictionary.Values.Where(u => u.IsOnline);
    }
}