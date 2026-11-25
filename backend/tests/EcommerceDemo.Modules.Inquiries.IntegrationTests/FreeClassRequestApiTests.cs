using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using EcommerceDemo.BuildingBlocks.Application;
using EcommerceDemo.Modules.Inquiries.Application.FreeClassRequests;
using EcommerceDemo.Modules.Users.Application.Auth;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

using Testcontainers.PostgreSql;

namespace EcommerceDemo.Modules.Inquiries.IntegrationTests;

public sealed class InquiriesApiFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public InquiriesApiFactory()
    {
        _postgres.StartAsync().GetAwaiter().GetResult();
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

public class FreeClassRequestApiTests : IClassFixture<InquiriesApiFactory>
{
    private readonly InquiriesApiFactory _factory;

    public FreeClassRequestApiTests(InquiriesApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Submit_persists_a_request_and_listing_requires_a_jwt()
    {
        var command = new SubmitFreeClassRequestCommand(
            "Ana Pérez",
            "0984204476",
            "Mestizo",
            "8 meses",
            "Tira de la correa en la calle");

        using var client = _factory.CreateClient();
        var created = await client.PostAsJsonAsync("/api/inquiries/free-classes", command);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var body = await created.Content.ReadFromJsonAsync<FreeClassRequestDto>();
        Assert.NotNull(body);
        Assert.Equal("Ana Pérez", body.OwnerName);
        Assert.Equal("Tira de la correa en la calle", body.BehaviorProblem);

        var anonymous = await client.GetAsync("/api/inquiries/free-classes");
        Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);

        var registered = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterUserCommand($"owner-{Guid.NewGuid():N}@example.com", "Testpass1", "Test Owner"));
        Assert.Equal(HttpStatusCode.OK, registered.StatusCode);
        var auth = await registered.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);

        using var authorized = _factory.CreateClient();
        authorized.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
        var listed = await authorized.GetAsync("/api/inquiries/free-classes?page=1&pageSize=20");
        Assert.Equal(HttpStatusCode.OK, listed.StatusCode);
        var page = await listed.Content.ReadFromJsonAsync<PagedResult<FreeClassRequestDto>>();
        Assert.NotNull(page);
        Assert.Contains(page.Items, item => item.Id == body.Id);
    }

    [Fact]
    public async Task Submit_rejects_a_request_without_a_behavior_problem()
    {
        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync(
            "/api/inquiries/free-classes",
            new SubmitFreeClassRequestCommand("Ana", "0984204476", "Mestizo", "2 años", " "));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("behaviorProblem", json, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Openapi_document_includes_the_free_class_path()
    {
        using var client = _factory.CreateClient();
        var json = await client.GetStringAsync("/openapi/v1.json");

        Assert.Contains("/api/inquiries/free-classes", json, StringComparison.Ordinal);
    }
}
