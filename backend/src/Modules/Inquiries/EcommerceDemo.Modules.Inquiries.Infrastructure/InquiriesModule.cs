using EcommerceDemo.BuildingBlocks.Application;
using EcommerceDemo.BuildingBlocks.Infrastructure.Persistence;
using EcommerceDemo.Modules.Inquiries.Application.FreeClassRequests;
using EcommerceDemo.Modules.Inquiries.Infrastructure.Persistence;

using FluentValidation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceDemo.Modules.Inquiries;

public static class InquiriesModule
{
    public static IServiceCollection AddInquiriesModule(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddModuleDbContext<InquiriesDbContext>(configuration, InquiriesDbContext.Schema);
        services.AddScoped<IFreeClassRequestRepository, EfFreeClassRequestRepository>();
        services.AddScoped<SubmitFreeClassRequestHandler>();
        services.AddScoped<ListFreeClassRequestsHandler>();
        services.AddValidatorsFromAssemblyContaining<SubmitFreeClassRequestCommandValidator>();
        return services;
    }

    public static async Task InitializeDatabaseAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<InquiriesDbContext>();
        await db.Database.MigrateAsync(cancellationToken);
    }

    public static IEndpointRouteBuilder MapInquiriesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var inquiries = endpoints.MapGroup("/api/inquiries").WithTags("Inquiries");

        inquiries.MapPost("/free-classes", async (
                SubmitFreeClassRequestCommand? command,
                SubmitFreeClassRequestHandler handler,
                CancellationToken cancellationToken) =>
            {
                if (command is null)
                {
                    return Results.ValidationProblem(new Dictionary<string, string[]>
                    {
                        ["body"] = ["A request body is required."],
                    });
                }

                var outcome = await handler.Handle(command, cancellationToken);
                return outcome switch
                {
                    SubmitFreeClassRequestResponse.Created created => Results.Created(
                        $"/api/inquiries/free-classes/{created.Request.Id}",
                        created.Request),
                    SubmitFreeClassRequestResponse.Invalid invalid => Results.ValidationProblem(invalid.Errors),
                    _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError),
                };
            })
            .WithName("SubmitFreeClassRequest")
            .WithSummary("Store a free training-class request from the public site.")
            .Produces<FreeClassRequestDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        inquiries.MapGet("/free-classes", async (
                ListFreeClassRequestsHandler handler,
                CancellationToken cancellationToken,
                int page = 1,
                int pageSize = 20) =>
            {
                var outcome = await handler.Handle(new ListFreeClassRequestsQuery(page, pageSize), cancellationToken);
                return outcome switch
                {
                    ListFreeClassRequestsResponse.Ok ok => Results.Ok(ok.Page),
                    ListFreeClassRequestsResponse.Invalid invalid => Results.ValidationProblem(invalid.Errors),
                    _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError),
                };
            })
            .WithName("ListFreeClassRequests")
            .WithSummary("List stored free-class requests. Requires the existing JWT; there is no admin screen yet.")
            .RequireAuthorization()
            .Produces<PagedResult<FreeClassRequestDto>>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return endpoints;
    }
}
