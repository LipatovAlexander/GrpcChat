using Chat;
using Grpc.Core;
using Grpc.Net.Client;

using var channel = GrpcChannel.ForAddress("https://localhost:5001/");

var chatClient = new ChatRoomService.ChatRoomServiceClient(channel);

var metadata = await AuthorizeAsync(chatClient);

var responseStream = chatClient.ReceiveMessages(new Empty(), metadata).ResponseStream;

_ = ReceiveMessagesAsync(responseStream);
await SendMessagesAsync(chatClient, metadata);

static async Task<Metadata> AuthorizeAsync(ChatRoomService.ChatRoomServiceClient client)
{
    Console.WriteLine("Enter your name:");

    var authRequest = new JoinRequest
    {
        User = Console.ReadLine()
    };

    var joinResponse = await client.JoinAsync(authRequest);

    Console.Clear();

    return new Metadata
    {
        {"Authorization", $"Bearer {joinResponse.Token}"}
    };
}

static async Task ReceiveMessagesAsync(IAsyncStreamReader<GetMessageResponse> stream)
{
    await foreach (var message in stream.ReadAllAsync())
    {
        Console.WriteLine($"{message.User}: {message.Text}");
    }
}

static async Task SendMessagesAsync(ChatRoomService.ChatRoomServiceClient client, Metadata metadata)
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
    
        await client.SendMessageAsync(new SendMessageRequest { Text = line }, metadata);
    }
}