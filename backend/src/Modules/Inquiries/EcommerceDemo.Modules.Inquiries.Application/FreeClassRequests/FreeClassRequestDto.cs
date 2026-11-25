namespace EcommerceDemo.Modules.Inquiries.Application.FreeClassRequests;

public sealed record FreeClassRequestDto(
    Guid Id,
    string OwnerName,
    string Phone,
    string Breed,
    string DogAge,
    string BehaviorProblem,
    DateTimeOffset CreatedAt);
