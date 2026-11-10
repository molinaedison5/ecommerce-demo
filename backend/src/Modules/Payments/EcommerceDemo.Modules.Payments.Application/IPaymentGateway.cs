using EcommerceDemo.BuildingBlocks.Domain;
using EcommerceDemo.Modules.Payments.Domain;

namespace EcommerceDemo.Modules.Payments.Application;

public sealed record PaymentAuthorizationRequest(string Reference, Money Amount, string Description);

public sealed record PaymentAuthorization(PaymentStatus Status, string? ProviderReference, string Message);

public sealed record PaymentProviderResponse(string Provider);

public interface IPaymentGateway
{
    string ProviderName { get; }

    Task<PaymentAuthorization> AuthorizeAsync(PaymentAuthorizationRequest request, CancellationToken cancellationToken);
}
