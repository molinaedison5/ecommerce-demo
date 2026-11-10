using EcommerceDemo.Modules.Payments.Application;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceDemo.Modules.Payments;

public static class PaymentsModule
{
    public static IServiceCollection AddPaymentsModule(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var provider = configuration["Payments:Provider"] ?? Infrastructure.FakePaymentGateway.Name;
        if (!provider.Equals(Infrastructure.FakePaymentGateway.Name, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Payments provider '{provider}' is not registered. Add an IPaymentGateway implementation (planned: Payphone, Kushki, PayPal).");
        }

        services.AddSingleton<IPaymentGateway, Infrastructure.FakePaymentGateway>();
        return services;
    }

    public static IEndpointRouteBuilder MapPaymentsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/payments/provider", (IPaymentGateway gateway) =>
                Results.Ok(new PaymentProviderResponse(gateway.ProviderName)))
            .WithTags("Payments")
            .WithName("GetPaymentProvider")
            .WithSummary("Names the configured payment provider. The bundled provider does not move money.")
            .Produces<PaymentProviderResponse>();

        return endpoints;
    }
}
