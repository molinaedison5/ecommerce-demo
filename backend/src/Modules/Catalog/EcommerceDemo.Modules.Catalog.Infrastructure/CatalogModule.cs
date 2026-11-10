using EcommerceDemo.BuildingBlocks.Application;
using EcommerceDemo.BuildingBlocks.Infrastructure.Persistence;
using EcommerceDemo.Modules.Catalog.Application.Products;
using EcommerceDemo.Modules.Catalog.Infrastructure.Caching;
using EcommerceDemo.Modules.Catalog.Infrastructure.Persistence;
using EcommerceDemo.Modules.Catalog.Infrastructure.Seeding;

using FluentValidation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceDemo.Modules.Catalog;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddModuleDbContext<CatalogDbContext>(configuration, CatalogDbContext.Schema);
        services.Configure<CatalogCacheOptions>(configuration.GetSection(CatalogCacheOptions.SectionName));
        services.AddScoped<EfProductReader>();
        services.AddScoped<IActiveProductSource>(provider => provider.GetRequiredService<EfProductReader>());
        services.AddScoped<IProductReader, CachedProductReader>();
        services.AddScoped<ListProductsHandler>();
        services.AddValidatorsFromAssemblyContaining<ListProductsQueryValidator>();
        return services;
    }

    public static async Task InitializeDatabaseAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await db.Database.MigrateAsync(cancellationToken);
        await CatalogSeeder.SeedAsync(db, cancellationToken);
    }

    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var catalog = endpoints.MapGroup("/api/catalog").WithTags("Catalog");

        catalog.MapGet("/products", async (
                ListProductsHandler handler,
                CancellationToken cancellationToken,
                int page = 1,
                int pageSize = 20) =>
            {
                var outcome = await handler.Handle(new ListProductsQuery(page, pageSize), cancellationToken);
                return outcome switch
                {
                    ListProductsResponse.Ok ok => Results.Ok(ok.Page),
                    ListProductsResponse.Invalid invalid => Results.ValidationProblem(invalid.Errors),
                    _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError),
                };
            })
            .WithName("ListCatalogProducts")
            .WithSummary("List active products in the pet-care catalog.")
            .Produces<PagedResult<ProductDto>>()
            .ProducesValidationProblem();

        return endpoints;
    }
}
