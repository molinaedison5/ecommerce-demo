using System.IdentityModel.Tokens.Jwt;
using System.Text;

using EcommerceDemo.BuildingBlocks.Infrastructure.Persistence;
using EcommerceDemo.Modules.Users.Application.Auth;
using EcommerceDemo.Modules.Users.Domain;
using EcommerceDemo.Modules.Users.Infrastructure.Auth;
using EcommerceDemo.Modules.Users.Infrastructure.Identity;

using FluentValidation;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceDemo.Modules.Users;

public static class UsersModule
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        jwt.EnsureValid();
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddModuleDbContext<UsersDbContext>(configuration, UsersDbContext.Schema);
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<UsersDbContext>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                    ClockSkew = TimeSpan.FromMinutes(1),
                };
            });
        services.AddAuthorization();

        services.AddScoped<IAccessTokenIssuer, JwtAccessTokenIssuer>();
        services.AddScoped<IAuthService, IdentityAuthService>();
        services.AddValidatorsFromAssemblyContaining<RegisterUserCommandValidator>();
        return services;
    }

    public static async Task InitializeDatabaseAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        await db.Database.MigrateAsync(cancellationToken);
    }

    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var auth = endpoints.MapGroup("/api/auth").WithTags("Auth");

        auth.MapPost("/register", async (RegisterUserCommand command, IAuthService authService, CancellationToken cancellationToken) =>
                ToHttpResult(await authService.RegisterAsync(command, cancellationToken)))
            .WithName("RegisterUser")
            .WithSummary("Create an account and return a JWT access token.")
            .Produces<AuthResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        auth.MapPost("/login", async (LoginCommand command, IAuthService authService, CancellationToken cancellationToken) =>
                ToHttpResult(await authService.LoginAsync(command, cancellationToken)))
            .WithName("Login")
            .WithSummary("Exchange email and password for a JWT access token.")
            .Produces<AuthResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        endpoints.MapGet("/api/users/me", (HttpContext http) =>
            {
                var principal = http.User;
                var idValue = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                if (!Guid.TryParse(idValue, out var id))
                {
                    return Results.Unauthorized();
                }

                var email = principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value ?? string.Empty;
                var displayName = principal.FindFirst("display_name")?.Value ?? string.Empty;
                return Results.Ok(new UserProfile(id, email, displayName));
            })
            .WithTags("Users")
            .WithName("GetCurrentUser")
            .RequireAuthorization()
            .Produces<UserProfile>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return endpoints;
    }

    private static IResult ToHttpResult(AuthOutcome outcome) => outcome.Status switch
    {
        AuthStatus.Success => Results.Ok(new AuthResponse(outcome.Token!.Value, "Bearer", outcome.Token.ExpiresInSeconds)),
        AuthStatus.Invalid => Results.ValidationProblem(outcome.Errors ?? new Dictionary<string, string[]>()),
        AuthStatus.Conflict => Results.Problem(
            title: "Account already exists",
            detail: outcome.Detail,
            statusCode: StatusCodes.Status409Conflict),
        AuthStatus.Unauthorized => Results.Problem(
            title: "Invalid credentials",
            statusCode: StatusCodes.Status401Unauthorized),
        _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError),
    };
}
