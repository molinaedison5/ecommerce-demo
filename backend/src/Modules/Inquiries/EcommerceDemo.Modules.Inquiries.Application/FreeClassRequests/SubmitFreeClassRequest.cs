using EcommerceDemo.BuildingBlocks.Application;
using EcommerceDemo.Modules.Inquiries.Domain.FreeClassRequests;

using FluentValidation;

namespace EcommerceDemo.Modules.Inquiries.Application.FreeClassRequests;

public sealed record SubmitFreeClassRequestCommand(
    string OwnerName,
    string Phone,
    string Breed,
    string DogAge,
    string BehaviorProblem);

public abstract record SubmitFreeClassRequestResponse
{
    private SubmitFreeClassRequestResponse()
    {
    }

    public sealed record Created(FreeClassRequestDto Request) : SubmitFreeClassRequestResponse;

    public sealed record Invalid(IDictionary<string, string[]> Errors) : SubmitFreeClassRequestResponse;
}

public sealed class SubmitFreeClassRequestCommandValidator : AbstractValidator<SubmitFreeClassRequestCommand>
{
    public SubmitFreeClassRequestCommandValidator()
    {
        RuleFor(command => command.OwnerName)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("Owner name is required.")
            .MaximumLength(FreeClassRequest.OwnerNameMaxLength)
            .OverridePropertyName("ownerName");

        RuleFor(command => command.Phone)
            .Must(FreeClassRequest.IsPlausiblePhone)
            .WithMessage("Phone must contain 7 to 15 digits and no letters.")
            .OverridePropertyName("phone");

        RuleFor(command => command.Breed)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("Breed is required.")
            .MaximumLength(FreeClassRequest.BreedMaxLength)
            .OverridePropertyName("breed");

        RuleFor(command => command.DogAge)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("Dog age is required.")
            .MaximumLength(FreeClassRequest.DogAgeMaxLength)
            .OverridePropertyName("dogAge");

        RuleFor(command => command.BehaviorProblem)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("Behavior problem is required.")
            .MaximumLength(FreeClassRequest.BehaviorProblemMaxLength)
            .OverridePropertyName("behaviorProblem");
    }
}

public interface IFreeClassRequestRepository
{
    Task AddAsync(FreeClassRequest request, CancellationToken cancellationToken);

    Task<PagedResult<FreeClassRequestDto>> ListAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}

public sealed class SubmitFreeClassRequestHandler(
    IFreeClassRequestRepository requests,
    IValidator<SubmitFreeClassRequestCommand> validator)
{
    public async Task<SubmitFreeClassRequestResponse> Handle(
        SubmitFreeClassRequestCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return new SubmitFreeClassRequestResponse.Invalid(validation.ToDictionary());
        }

        var request = FreeClassRequest.Create(
            Guid.NewGuid(),
            command.OwnerName,
            command.Phone,
            command.Breed,
            command.DogAge,
            command.BehaviorProblem,
            DateTimeOffset.UtcNow);

        await requests.AddAsync(request, cancellationToken);
        return new SubmitFreeClassRequestResponse.Created(ToDto(request));
    }

    internal static FreeClassRequestDto ToDto(FreeClassRequest request) =>
        new(request.Id, request.OwnerName, request.Phone, request.Breed, request.DogAge, request.BehaviorProblem, request.CreatedAt);
}
