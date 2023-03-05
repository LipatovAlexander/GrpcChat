using System.Collections.Concurrent;
using Chat;

namespace Server.Models;

public sealed class User
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public bool IsOnline { get; set; }
    private ConcurrentQueue<GetMessageResponse> MessageQueue { get; } = new();

    public void SendMessage(GetMessageResponse message) => MessageQueue.Enqueue(message);

    public IEnumerable<GetMessageResponse> GetMessages()
    {
        while (MessageQueue.TryDequeue(out var message))
        {
            yield return message;
        }
    }
}