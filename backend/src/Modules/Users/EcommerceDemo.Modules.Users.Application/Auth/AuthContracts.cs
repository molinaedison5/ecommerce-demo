namespace EcommerceDemo.Modules.Users.Application.Auth;

public sealed record RegisterUserCommand(string Email, string Password, string DisplayName);

public sealed record LoginCommand(string Email, string Password);

public sealed record AccessToken(string Value, DateTimeOffset ExpiresAt, int ExpiresInSeconds);

public sealed record AuthResponse(string AccessToken, string TokenType, int ExpiresIn);

public enum AuthStatus
{
    Success = 1,
    Invalid = 2,
    Conflict = 3,
    Unauthorized = 4,
}

public sealed record AuthOutcome(AuthStatus Status, AccessToken? Token, IDictionary<string, string[]>? Errors, string? Detail)
{
    public static AuthOutcome Success(AccessToken token) => new(AuthStatus.Success, token, null, null);

    public static AuthOutcome Invalid(IDictionary<string, string[]> errors) => new(AuthStatus.Invalid, null, errors, null);

    public static AuthOutcome Conflict(string detail) => new(AuthStatus.Conflict, null, null, detail);

    public static AuthOutcome Unauthorized() => new(AuthStatus.Unauthorized, null, null, null);
}

public interface IAccessTokenIssuer
{
    AccessToken Issue(Guid userId, string email, string displayName);
}

public interface IAuthService
{
    Task<AuthOutcome> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken);

    Task<AuthOutcome> LoginAsync(LoginCommand command, CancellationToken cancellationToken);
}
