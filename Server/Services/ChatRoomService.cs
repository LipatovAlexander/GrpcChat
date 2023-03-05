using System.Security.Claims;
using Chat;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Server.Models;
using Server.Repositories;

namespace Server.Services;

public sealed class ChatRoomService : Chat.ChatRoomService.ChatRoomServiceBase
{
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IUserRepository _userRepository;

    public ChatRoomService(IJwtGenerator jwtGenerator, IUserRepository userRepository)
    {
        _jwtGenerator = jwtGenerator;
        _userRepository = userRepository;
    }

    public override async Task<JoinResponse> Join(JoinRequest request, ServerCallContext context)
    {
        var cancellationToken = context.CancellationToken;
    
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = request.User,
            IsOnline = false
        };
    
        var response = new JoinResponse
        {
            Token = _jwtGenerator.Generate(user)
        };

        await _userRepository.AddAsync(user, cancellationToken);
        
        return response;
    }

    [Authorize]
    public override async Task<Empty> SendMessage(SendMessageRequest request, ServerCallContext context)
    {
        var cancellationToken = context.CancellationToken;
    
        var httpContext = context.GetHttpContext();
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return new Empty();
        }

        var allUsers = await _userRepository.GetAllOnlineAsync(cancellationToken);

        var response = new GetMessageResponse
        {
            User = user.UserName,
            Text = request.Text
        };
        
        foreach (var receiver in allUsers)
        {
            receiver.MessageQueue.Enqueue(response);
        }

        return new Empty();
    }

    [Authorize]
    public override async Task ReceiveMessages(Empty request, IServerStreamWriter<GetMessageResponse> responseStream, ServerCallContext context)
    {
        var cancellationToken = context.CancellationToken;
    
        var httpContext = context.GetHttpContext();
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return;
        }

        user.IsOnline = true;
        await _userRepository.UpdateAsync(user, cancellationToken);

        var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                while (user.MessageQueue.TryDequeue(out var message))
                {
                    await responseStream.WriteAsync(message, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        
        user.IsOnline = false;
        await _userRepository.UpdateAsync(user, cancellationToken);
    }
}