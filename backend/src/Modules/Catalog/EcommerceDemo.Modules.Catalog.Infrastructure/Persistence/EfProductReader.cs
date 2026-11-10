using EcommerceDemo.BuildingBlocks.Application;
using EcommerceDemo.Modules.Catalog.Application.Products;

using Microsoft.EntityFrameworkCore;

namespace EcommerceDemo.Modules.Catalog.Infrastructure.Persistence;

internal interface IActiveProductSource
{
    Task<PagedResult<ProductDto>> ListActiveAsync(int page, int pageSize, CancellationToken cancellationToken);
}

internal sealed class EfProductReader(CatalogDbContext db) : IActiveProductSource
{
    public async Task<PagedResult<ProductDto>> ListActiveAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = db.Products.AsNoTracking().Where(product => product.IsActive);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(product => product.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(product => new ProductDto(
                product.Id,
                product.Name,
                product.Description,
                product.PriceAmount,
                product.PriceCurrency,
                product.Sku))
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductDto>(items, page, pageSize, total);
    }
}
