using EcommerceDemo.BuildingBlocks.Application;

using FluentValidation;

namespace EcommerceDemo.Modules.Catalog.Application.Products;

public sealed record ListProductsQuery(int Page, int PageSize);

public abstract record ListProductsResponse
{
    private ListProductsResponse()
    {
    }

    public sealed record Ok(PagedResult<ProductDto> Page) : ListProductsResponse;

    public sealed record Invalid(IDictionary<string, string[]> Errors) : ListProductsResponse;
}

public sealed class ListProductsQueryValidator : AbstractValidator<ListProductsQuery>
{
    public const int MaxPageSize = 100;

    public ListProductsQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, MaxPageSize);
    }
}

public interface IProductReader
{
    Task<PagedResult<ProductDto>> ListActiveAsync(int page, int pageSize, CancellationToken cancellationToken);
}

public sealed class ListProductsHandler(IProductReader products, IValidator<ListProductsQuery> validator)
{
    public async Task<ListProductsResponse> Handle(ListProductsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            return new ListProductsResponse.Invalid(validation.ToDictionary());
        }

        var page = await products.ListActiveAsync(query.Page, query.PageSize, cancellationToken);
        return new ListProductsResponse.Ok(page);
    }
}
