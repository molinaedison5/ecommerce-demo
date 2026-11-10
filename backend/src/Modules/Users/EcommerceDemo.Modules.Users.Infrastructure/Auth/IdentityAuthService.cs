using EcommerceDemo.Modules.Users.Application.Auth;
using EcommerceDemo.Modules.Users.Infrastructure.Identity;

using FluentValidation;

using Microsoft.AspNetCore.Identity;

namespace EcommerceDemo.Modules.Users.Infrastructure.Auth;

public sealed class IdentityAuthService(
    UserManager<ApplicationUser> users,
    IAccessTokenIssuer tokens,
    IValidator<RegisterUserCommand> registerValidator,
    IValidator<LoginCommand> loginValidator) : IAuthService
{
    public async Task<AuthOutcome> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        var validation = await registerValidator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return AuthOutcome.Invalid(validation.ToDictionary());
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = command.Email.Trim(),
            Email = command.Email.Trim(),
            DisplayName = command.DisplayName.Trim(),
            EmailConfirmed = true,
        };

        var result = await users.CreateAsync(user, command.Password);
        if (!result.Succeeded)
        {
            if (result.Errors.Any(error => error.Code is "DuplicateEmail" or "DuplicateUserName"))
            {
                return AuthOutcome.Conflict("An account with that email already exists.");
            }

            var errors = result.Errors
                .GroupBy(error => error.Code)
                .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());
            return AuthOutcome.Invalid(errors);
        }

        return AuthOutcome.Success(tokens.Issue(user.Id, user.Email, user.DisplayName));
    }

    public async Task<AuthOutcome> LoginAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        var validation = await loginValidator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return AuthOutcome.Invalid(validation.ToDictionary());
        }

        var user = await users.FindByEmailAsync(command.Email.Trim());
        if (user is null || !await users.CheckPasswordAsync(user, command.Password))
        {
            return AuthOutcome.Unauthorized();
        }

        return AuthOutcome.Success(tokens.Issue(user.Id, user.Email ?? command.Email.Trim(), user.DisplayName));
    }
}
