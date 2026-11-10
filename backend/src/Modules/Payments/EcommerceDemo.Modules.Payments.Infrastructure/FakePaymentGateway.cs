using EcommerceDemo.Modules.Payments.Application;
using EcommerceDemo.Modules.Payments.Domain;

namespace EcommerceDemo.Modules.Payments.Infrastructure;

public sealed class FakePaymentGateway : IPaymentGateway
{
    public const string Name = "fake";

    public string ProviderName => Name;

    public Task<PaymentAuthorization> AuthorizeAsync(PaymentAuthorizationRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(request.Reference))
        {
            throw new ArgumentException("Reference is required.", nameof(request));
        }

        if (request.Amount.Amount <= 0)
        {
            return Task.FromResult(new PaymentAuthorization(
                PaymentStatus.Declined,
                null,
                "Amount must be greater than zero."));
        }

        return Task.FromResult(new PaymentAuthorization(
            PaymentStatus.Authorized,
            $"fake_{Guid.NewGuid():N}",
            "Authorized by the fake gateway. No charge was made."));
    }
}
