using EcommerceDemo.BuildingBlocks.Infrastructure.Caching;
using EcommerceDemo.BuildingBlocks.Infrastructure.Health;
using EcommerceDemo.Modules.Bookings;
using EcommerceDemo.Modules.Cart;
using EcommerceDemo.Modules.Catalog;
using EcommerceDemo.Modules.Orders;
using EcommerceDemo.Modules.Payments;
using EcommerceDemo.Modules.Users;

using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console(new CompactJsonFormatter());
});

builder.Services.AddAppCaching(builder.Configuration);
builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddBookingsModule(builder.Configuration);
builder.Services.AddCartModule(builder.Configuration);
builder.Services.AddOrdersModule(builder.Configuration);
builder.Services.AddPaymentsModule(builder.Configuration);
builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddInfrastructureHealthChecks(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "Ecommerce Demo API";
        document.Info.Version = "v1";
        document.Info.Description =
            "Pet-care commerce API. Modules: Catalog, Bookings, Cart, Orders, Payments, Users. Payments use IPaymentGateway; the bundled provider is fake.";
        return Task.CompletedTask;
    });
});

var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(corsOrigins).AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if (app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
{
    await InitializeDatabasesAsync(app);
}

app.UseExceptionHandler();
app.UseSerilogRequestLogging();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Ecommerce Demo v1");
    options.RoutePrefix = "swagger";
});

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapHealthChecks("/health");
app.MapCatalogEndpoints();
app.MapBookingsEndpoints();
app.MapCartEndpoints();
app.MapOrdersEndpoints();
app.MapPaymentsEndpoints();
app.MapUsersEndpoints();

app.Run();

static async Task InitializeDatabasesAsync(WebApplication app)
{
    const int attempts = 8;
    for (var attempt = 1; ; attempt++)
    {
        try
        {
            app.Logger.LogInformation("Applying module database migrations");
            await CatalogModule.InitializeDatabaseAsync(app.Services);
            await UsersModule.InitializeDatabaseAsync(app.Services);
            return;
        }
        catch (Exception exception) when (attempt < attempts)
        {
            app.Logger.LogWarning(
                exception,
                "Database initialization attempt {Attempt} of {Attempts} failed",
                attempt,
                attempts);
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}

public partial class Program;
