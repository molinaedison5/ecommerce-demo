using EcommerceDemo.BuildingBlocks.Domain;
using EcommerceDemo.Modules.Payments.Application;
using EcommerceDemo.Modules.Payments.Domain;
using EcommerceDemo.Modules.Payments.Infrastructure;

namespace EcommerceDemo.Modules.Payments.UnitTests;

public class FakePaymentGatewayTests
{
    private readonly FakePaymentGateway _gateway = new();

    [Fact]
    public async Task Authorize_returns_a_provider_reference_without_charging()
    {
        var result = await _gateway.AuthorizeAsync(
            new PaymentAuthorizationRequest("order-1", new Money(18.50m, "usd"), "Daycare deposit"),
            CancellationToken.None);

        Assert.Equal(PaymentStatus.Authorized, result.Status);
        Assert.Equal("fake", _gateway.ProviderName);
        Assert.StartsWith("fake_", result.ProviderReference, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Authorize_declines_a_zero_amount()
    {
        var result = await _gateway.AuthorizeAsync(
            new PaymentAuthorizationRequest("order-2", new Money(0m, "USD"), "Empty cart"),
            CancellationToken.None);

        Assert.Equal(PaymentStatus.Declined, result.Status);
        Assert.Null(result.ProviderReference);
    }
}
