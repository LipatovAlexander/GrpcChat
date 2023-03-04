using Chat;
using Grpc.Core;
using Grpc.Net.Client;

using var channel = GrpcChannel.ForAddress("http://localhost:5221/");

var metadata = await AuthorizeAsync(channel);

using var chat = new ChatRoomService.ChatRoomServiceClient(channel).Join(metadata);

var responseStream = chat.ResponseStream;
var requestStream = chat.RequestStream;

_ = ReceiveMessagesAsync(responseStream);
await SendMessagesAsync(requestStream);
await chat.RequestStream.CompleteAsync();

static async Task<Metadata> AuthorizeAsync(ChannelBase channel)
{
    var authClient = new AuthService.AuthServiceClient(channel);

    Console.WriteLine("Enter your name:");

    var authRequest = new AuthRequest
    {
        UserName = Console.ReadLine()
    };

    var authResponse = await authClient.AuthenticateAsync(authRequest);

    Console.Clear();

    return new Metadata
    {
        {"Authorization", $"Bearer {authResponse.Token}"}
    };
}

static async Task ReceiveMessagesAsync(IAsyncStreamReader<GetMessageResponse> stream)
{
    await foreach (var message in stream.ReadAllAsync())
    {
        Console.WriteLine($"{message.User}: {message.Text}");
    }
}

static async Task SendMessagesAsync(IAsyncStreamWriter<SendMessageRequest> stream)
{
    while (Console.ReadLine() is { } line)
    {
        if (line.ToLower() == "bye")
        {
            break;
        }
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        Console.Write(new string(' ', Console.BufferWidth));
        Console.SetCursorPosition(0, Console.CursorTop);
    
        await stream.WriteAsync(new SendMessageRequest { Text = line });
    }
}