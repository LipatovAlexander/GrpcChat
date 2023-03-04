using System.Security.Claims;
using Chat;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Server.Models;

namespace Server.Services;

[Authorize]
public sealed class ChatRoomService : Chat.ChatRoomService.ChatRoomServiceBase
{
    private readonly IChatRoom _chatRoom;

    public ChatRoomService(IChatRoom chatRoom)
    {
        _chatRoom = chatRoom;
    }

    public override async Task Join(
        IAsyncStreamReader<SendMessageRequest> requestStream,
        IServerStreamWriter<GetMessageResponse> responseStream,
        ServerCallContext context)
    {
        var cancellationToken = context.CancellationToken;
    
        var httpContext = context.GetHttpContext();
        var userName = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = new User
        {
            UserName = userName ?? "Unknown",
            ResponseStream = responseStream
        };

        await _chatRoom.JoinAsync(user, cancellationToken);

        await foreach (var message in requestStream.ReadAllAsync(cancellationToken))
        {
            await _chatRoom.BroadCastMessageAsync(user, message, cancellationToken);
        }

        await _chatRoom.LeaveAsync(user, cancellationToken);
    }
}