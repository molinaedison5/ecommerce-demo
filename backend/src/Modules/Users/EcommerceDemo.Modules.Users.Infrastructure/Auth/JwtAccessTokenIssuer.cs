using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using EcommerceDemo.Modules.Users.Application.Auth;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceDemo.Modules.Users.Infrastructure.Auth;

public sealed class JwtAccessTokenIssuer(IOptions<JwtOptions> options) : IAccessTokenIssuer
{
    public AccessToken Issue(Guid userId, string email, string displayName)
    {
        var jwt = options.Value;
        jwt.EnsureValid();

        var expires = DateTimeOffset.UtcNow.AddMinutes(jwt.ExpirationMinutes);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt.Issuer,
            audience: jwt.Audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("display_name", displayName),
            ],
            expires: expires.UtcDateTime,
            signingCredentials: credentials);

        var value = new JwtSecurityTokenHandler().WriteToken(token);
        return new AccessToken(value, expires, jwt.ExpirationMinutes * 60);
    }
}
