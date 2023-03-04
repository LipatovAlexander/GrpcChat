using Server;
using Server.Repositories;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var services = builder.Services;

var jwtConfig = config.GetSection(JwtSettings.SectionName);

services
    .AddOptions<JwtSettings>()
    .Bind(jwtConfig)
    .ValidateDataAnnotations()
    .ValidateOnStart();

var jwtSettings = new JwtSettings();
jwtConfig.Bind(jwtSettings);

services.AddGrpc();

services.AddJwtAuthorization(jwtSettings);

services.AddSingleton<IChatRoom, ChatRoom>();
services.AddSingleton<IUserRepository, UserRepository>();

var app = builder.Build();

app.UseJwtAuthorization();

app.MapGrpcService<ChatRoomService>();
app.MapGrpcService<AuthService>();

app.Run();