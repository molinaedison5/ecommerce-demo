using System.Text.Json;

using EcommerceDemo.BuildingBlocks.Application;
using EcommerceDemo.Modules.Catalog.Application.Products;
using EcommerceDemo.Modules.Catalog.Infrastructure.Persistence;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EcommerceDemo.Modules.Catalog.Infrastructure.Caching;

public sealed class CatalogCacheOptions
{
    public const string SectionName = "Cache";

    public int CatalogTtlSeconds { get; set; } = 60;
}

internal sealed class CachedProductReader(
    IActiveProductSource inner,
    IDistributedCache cache,
    IOptions<CatalogCacheOptions> options,
    ILogger<CachedProductReader> logger) : IProductReader
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<PagedResult<ProductDto>> ListActiveAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var ttl = options.Value.CatalogTtlSeconds;
        if (ttl <= 0)
        {
            return await inner.ListActiveAsync(page, pageSize, cancellationToken);
        }

        var key = $"catalog:products:p{page}:s{pageSize}";
        try
        {
            var cached = await cache.GetStringAsync(key, cancellationToken);
            if (cached is not null)
            {
                var hit = JsonSerializer.Deserialize<CacheEntry>(cached, JsonOptions);
                if (hit is not null)
                {
                    return new PagedResult<ProductDto>(hit.Items, hit.Page, hit.PageSize, hit.TotalCount);
                }
            }
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Catalog cache read failed for {CacheKey}", key);
        }

        var result = await inner.ListActiveAsync(page, pageSize, cancellationToken);
        try
        {
            var entry = new CacheEntry(result.Items.ToList(), result.Page, result.PageSize, result.TotalCount);
            await cache.SetStringAsync(
                key,
                JsonSerializer.Serialize(entry, JsonOptions),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttl) },
                cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Catalog cache write failed for {CacheKey}", key);
        }

        return result;
    }

    private sealed record CacheEntry(List<ProductDto> Items, int Page, int PageSize, int TotalCount);
}
