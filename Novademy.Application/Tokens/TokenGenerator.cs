using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Novademy.Application.Models;

namespace Novademy.Application.Tokens;

public class TokenGenerator : ITokenGenerator
{
    private readonly JwtOptions _jwtOptions;
    
    public TokenGenerator(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }
    
    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new("sub", user.Username),
            new("id", user.Id.ToString()),
            new("role", user.Role!.Name),
            new("firstName", user.FirstName),
            new("lastName", user.LastName),
            new("email", user.Email),
            new("phoneNumber", user.PhoneNumber),
            new("group", user.Group.ToString()),
            new("sector", ((int)user.Sector).ToString()),
        };
        
        // Include profile picture URL if available
        if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
        {
            claims.Add(new Claim("profilePictureUrl", user.ProfilePictureUrl!));
        }
        
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.Add(_jwtOptions.AccessValidFor),
            signingCredentials: _jwtOptions.SigningCredentials,
            claims: claims
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string GenerateRefreshToken() => Guid.NewGuid().ToString();
}