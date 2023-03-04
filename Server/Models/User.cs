using Chat;
using Grpc.Core;

namespace Server.Models;

public sealed class User
{
    public required string UserName { get; set; }
    public required IServerStreamWriter<GetMessageResponse> ResponseStream { get; set; }
}