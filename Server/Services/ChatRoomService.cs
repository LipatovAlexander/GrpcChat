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

    public override Task<JoinResponse> Join(JoinRequest request, ServerCallContext context)
    {
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

        _userRepository.Add(user);
        
        return Task.FromResult(response);
    }

    [Authorize]
    public override Task<Empty> SendMessage(SendMessageRequest request, ServerCallContext context)
    {
        var httpContext = context.GetHttpContext();
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";

        var user = _userRepository.GetById(userId);

        if (user is null)
        {
            return Task.FromResult(new Empty());
        }

        var response = new GetMessageResponse
        {
            User = user.UserName,
            Text = request.Text
        };
        
        foreach (var receiver in _userRepository.GetAllOnline())
        {
            receiver.SendMessage(response);
        }

        return Task.FromResult(new Empty());
    }

    [Authorize]
    public override async Task ReceiveMessages(Empty request, IServerStreamWriter<GetMessageResponse> responseStream, ServerCallContext context)
    {
        var cancellationToken = context.CancellationToken;
    
        var httpContext = context.GetHttpContext();
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";

        var user = _userRepository.GetById(userId);

        if (user is null)
        {
            return;
        }

        user.IsOnline = true;

        var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                foreach (var message in user.GetMessages())
                {
                    await responseStream.WriteAsync(message, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        
        user.IsOnline = false;
    }
}