using System.Collections.Concurrent;
using Chat;
using Grpc.Core;

namespace Server.Models;

public sealed class User
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public bool IsOnline { get; set; }
    public ConcurrentQueue<GetMessageResponse> MessageQueue { get; set; } = new();
}