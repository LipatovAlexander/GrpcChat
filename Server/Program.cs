using Server;
using Server.Repositories;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureGrpcWebHosting();

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

services.AddGrpcWebCors();

services.AddSingleton<IUserRepository, UserRepository>();
services.AddSingleton<IJwtGenerator, JwtGenerator>();

var app = builder.Build();

app.UseGrpcWeb();
app.UseCors();

app.UseJwtAuthorization();

app.MapGrpcService<ChatRoomService>()
    .EnableGrpcWeb()
    .RequireCors("AllowAll");

app.Run();