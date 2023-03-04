using Chat;
using Server.Models;
using Server.Repositories;

namespace Server;

public interface IChatRoom
{
    Task JoinAsync(User user, CancellationToken cancellationToken);
    Task LeaveAsync(User user, CancellationToken cancellationToken);
    Task BroadCastMessageAsync(User user, SendMessageRequest request, CancellationToken cancellationToken);
}

public sealed class ChatRoom : IChatRoom
{
    private readonly IUserRepository _userRepository;

    public ChatRoom(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task JoinAsync(User user, CancellationToken cancellationToken)
    {
        await _userRepository.AddAsync(user, cancellationToken);

        await BroadCastMessageAsync(user, new SendMessageRequest
        {
            Text = "Joined the room"
        }, cancellationToken);
    }

    public async Task LeaveAsync(User user, CancellationToken cancellationToken)
    {
        await _userRepository.RemoveAsync(user, cancellationToken);
        
        await BroadCastMessageAsync(user, new SendMessageRequest
        {
            Text = "Left the room"
        }, cancellationToken);
    }

    public async Task BroadCastMessageAsync(User user, SendMessageRequest request, CancellationToken cancellationToken)
    {
        var response = new GetMessageResponse
        {
            User = user.UserName,
            Text = request.Text
        };
    
        var allUsers = await _userRepository.GetAllAsync(cancellationToken);
        foreach (var receiver in allUsers)
        {
            await receiver.ResponseStream.WriteAsync(response, cancellationToken);
        }
    }
}