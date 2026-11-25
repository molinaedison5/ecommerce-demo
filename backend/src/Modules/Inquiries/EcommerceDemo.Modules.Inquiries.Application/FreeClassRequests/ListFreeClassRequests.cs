using EcommerceDemo.BuildingBlocks.Application;

using FluentValidation;

namespace EcommerceDemo.Modules.Inquiries.Application.FreeClassRequests;

public sealed record ListFreeClassRequestsQuery(int Page, int PageSize);

public abstract record ListFreeClassRequestsResponse
{
    private ListFreeClassRequestsResponse()
    {
    }

    public sealed record Ok(PagedResult<FreeClassRequestDto> Page) : ListFreeClassRequestsResponse;

    public sealed record Invalid(IDictionary<string, string[]> Errors) : ListFreeClassRequestsResponse;
}

public sealed class ListFreeClassRequestsQueryValidator : AbstractValidator<ListFreeClassRequestsQuery>
{
    public const int MaxPageSize = 100;

    public ListFreeClassRequestsQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, MaxPageSize);
    }
}

public sealed class ListFreeClassRequestsHandler(
    IFreeClassRequestRepository requests,
    IValidator<ListFreeClassRequestsQuery> validator)
{
    public async Task<ListFreeClassRequestsResponse> Handle(
        ListFreeClassRequestsQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            return new ListFreeClassRequestsResponse.Invalid(validation.ToDictionary());
        }

        var page = await requests.ListAsync(query.Page, query.PageSize, cancellationToken);
        return new ListFreeClassRequestsResponse.Ok(page);
    }
}
