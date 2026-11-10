using System.IdentityModel.Tokens.Jwt;

using EcommerceDemo.Modules.Users.Application.Auth;
using EcommerceDemo.Modules.Users.Infrastructure.Auth;

using Microsoft.Extensions.Options;

namespace EcommerceDemo.Modules.Users.UnitTests;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator = new();

    [Fact]
    public void Rejects_an_invalid_email_and_a_weak_password()
    {
        var result = _validator.Validate(new RegisterUserCommand("not-an-email", "short", "Ada"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Email");
        Assert.Contains(result.Errors, error => error.PropertyName == "Password");
    }

    [Fact]
    public void Accepts_a_password_that_matches_the_identity_rules()
    {
        var result = _validator.Validate(new RegisterUserCommand("ada@example.com", "Sup3rSecret", "Ada Lovelace"));

        Assert.True(result.IsValid);
    }
}

public class JwtAccessTokenIssuerTests
{
    [Fact]
    public void Issue_embeds_the_user_id_email_and_display_name()
    {
        var userId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        var issuer = new JwtAccessTokenIssuer(Options.Create(new JwtOptions
        {
            Issuer = "ecommerce-demo-tests",
            Audience = "ecommerce-demo-tests",
            SigningKey = "testing-signing-key-at-least-32-characters",
            ExpirationMinutes = 15,
        }));

        var token = issuer.Issue(userId, "ada@example.com", "Ada");
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.Value);

        Assert.Equal(userId.ToString(), jwt.Subject);
        Assert.Equal("ada@example.com", jwt.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Equal("Ada", jwt.Claims.Single(claim => claim.Type == "display_name").Value);
        Assert.Equal(15 * 60, token.ExpiresInSeconds);
    }
}
