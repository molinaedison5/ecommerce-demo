using System.Net;
using System.Net.Http.Json;

using EcommerceDemo.BuildingBlocks.Application;
using EcommerceDemo.Modules.Catalog.Application.Products;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

using Testcontainers.PostgreSql;

namespace EcommerceDemo.Modules.Catalog.IntegrationTests;

public sealed class CatalogApiFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public CatalogApiFactory()
    {
        _postgres.StartAsync().GetAwaiter().GetResult();
        // Environment variables outrank appsettings. WebApplicationFactory's in-memory
        // configuration is applied too early to override the host's JSON sources.
        Set("ConnectionStrings__Postgres", _postgres.GetConnectionString());
        Set("Cache__Provider", "Memory");
        Set("Database__ApplyMigrationsOnStartup", "true");
        Set("Payments__Provider", "fake");
        Set("Jwt__Issuer", "ecommerce-demo-tests");
        Set("Jwt__Audience", "ecommerce-demo-tests");
        Set("Jwt__SigningKey", "testing-signing-key-at-least-32-characters");
        Set("Jwt__ExpirationMinutes", "15");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }

    public override async ValueTask DisposeAsync()
    {
        Clear("ConnectionStrings__Postgres");
        Clear("Cache__Provider");
        Clear("Database__ApplyMigrationsOnStartup");
        Clear("Payments__Provider");
        Clear("Jwt__Issuer");
        Clear("Jwt__Audience");
        Clear("Jwt__SigningKey");
        Clear("Jwt__ExpirationMinutes");
        await base.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private static void Set(string name, string value) => Environment.SetEnvironmentVariable(name, value);

    private static void Clear(string name) => Environment.SetEnvironmentVariable(name, null);
}

public class CatalogApiTests : IClassFixture<CatalogApiFactory>
{
    private readonly HttpClient _client;

    public CatalogApiTests(CatalogApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task List_products_returns_the_seeded_catalog()
    {
        var response = await _client.GetAsync("/api/catalog/products?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();
        Assert.NotNull(body);
        Assert.Equal(4, body.TotalCount);
        Assert.Contains(body.Items, product => product.Sku == "DOG-FOOD-001");
        Assert.Contains(body.Items, product => product.Name == "Arena aglomerante para gatos");
    }

    [Fact]
    public async Task List_products_rejects_a_page_outside_the_contract()
    {
        var response = await _client.GetAsync("/api/catalog/products?page=0&pageSize=10");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Openapi_document_includes_the_catalog_path()
    {
        var json = await _client.GetStringAsync("/openapi/v1.json");

        Assert.Contains("/api/catalog/products", json, StringComparison.Ordinal);
    }
}
