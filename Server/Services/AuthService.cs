using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Chat;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Server.Services;

public sealed class AuthService : Chat.AuthService.AuthServiceBase
{
    private readonly JwtSettings _jwtSettings;

    public AuthService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public override Task<AuthResponse> Authenticate(AuthRequest request, ServerCallContext context)
    {
        var response = new AuthResponse
        {
            Token = GenerateToken(request)
        };
        
        return Task.FromResult(response);
    }
    
    private string GenerateToken(AuthRequest user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, user.UserName)
        };
        var token = new JwtSecurityToken(_jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.Now.AddHours(24),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}