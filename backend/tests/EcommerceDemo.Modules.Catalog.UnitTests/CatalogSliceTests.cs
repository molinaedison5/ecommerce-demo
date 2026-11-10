using EcommerceDemo.BuildingBlocks.Application;
using EcommerceDemo.Modules.Catalog.Application.Products;
using EcommerceDemo.Modules.Catalog.Domain.Products;
using EcommerceDemo.Modules.Catalog.Infrastructure.Caching;
using EcommerceDemo.Modules.Catalog.Infrastructure.Persistence;

using FluentValidation;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace EcommerceDemo.Modules.Catalog.UnitTests;

public class ProductTests
{
    [Fact]
    public void Create_rejects_a_negative_price()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            Product.Create(Guid.NewGuid(), "SKU-1", "Name", "Description", -1m, "USD"));

        Assert.Equal("amount", exception.ParamName);
    }

    [Fact]
    public void Create_normalizes_sku_and_currency()
    {
        var product = Product.Create(Guid.NewGuid(), " dog-food-001 ", " Alimento ", " Bolsa ", 24.9m, "usd");

        Assert.Equal("DOG-FOOD-001", product.Sku);
        Assert.Equal("USD", product.PriceCurrency);
        Assert.Equal(24.90m, product.PriceAmount);
        Assert.True(product.IsActive);
    }
}

public class ListProductsQueryValidatorTests
{
    private readonly ListProductsQueryValidator _validator = new();

    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    public void Rejects_page_values_outside_the_contract(int page, int pageSize)
    {
        var result = _validator.Validate(new ListProductsQuery(page, pageSize));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Accepts_the_default_window()
    {
        var result = _validator.Validate(new ListProductsQuery(1, 20));

        Assert.True(result.IsValid);
    }
}

public class ListProductsHandlerTests
{
    [Fact]
    public async Task Does_not_read_products_when_the_query_is_invalid()
    {
        var reader = new CountingReader();
        var handler = new ListProductsHandler(reader, new ListProductsQueryValidator());

        var outcome = await handler.Handle(new ListProductsQuery(0, 20), CancellationToken.None);

        Assert.IsType<ListProductsResponse.Invalid>(outcome);
        Assert.Equal(0, reader.Calls);
    }

    [Fact]
    public async Task Returns_the_reader_page_when_the_query_is_valid()
    {
        var reader = new CountingReader();
        var handler = new ListProductsHandler(reader, new ListProductsQueryValidator());

        var outcome = await handler.Handle(new ListProductsQuery(1, 10), CancellationToken.None);

        var ok = Assert.IsType<ListProductsResponse.Ok>(outcome);
        Assert.Equal(1, reader.Calls);
        Assert.Equal("CAT-LITTER-001", ok.Page.Items[0].Sku);
    }

    private sealed class CountingReader : IProductReader
    {
        public int Calls { get; private set; }

        public Task<PagedResult<ProductDto>> ListActiveAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            Calls++;
            IReadOnlyList<ProductDto> items =
            [
                new(Guid.Parse("6f1c1a10-1f4a-4c0e-9b3a-222222222222"), "Arena aglomerante para gatos", "Arena", 12.50m, "USD", "CAT-LITTER-001"),
            ];
            return Task.FromResult(new PagedResult<ProductDto>(items, page, pageSize, 1));
        }
    }
}

public class CachedProductReaderTests
{
    [Fact]
    public async Task Second_read_uses_the_cache()
    {
        var inner = new CountingSource();
        var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var reader = new CachedProductReader(
            inner,
            cache,
            Options.Create(new CatalogCacheOptions { CatalogTtlSeconds = 30 }),
            NullLogger<CachedProductReader>.Instance);

        var first = await reader.ListActiveAsync(1, 10, CancellationToken.None);
        var second = await reader.ListActiveAsync(1, 10, CancellationToken.None);

        Assert.Equal(1, inner.Calls);
        Assert.Equal(first.TotalCount, second.TotalCount);
        Assert.Equal(first.Items[0].Sku, second.Items[0].Sku);
    }

    private sealed class CountingSource : IActiveProductSource
    {
        public int Calls { get; private set; }

        public Task<PagedResult<ProductDto>> ListActiveAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            Calls++;
            IReadOnlyList<ProductDto> items =
            [
                new(Guid.NewGuid(), "Cepillo", "Doble cara", 9.99m, "USD", "GROOM-BRUSH-001"),
            ];
            return Task.FromResult(new PagedResult<ProductDto>(items, page, pageSize, 1));
        }
    }
}
