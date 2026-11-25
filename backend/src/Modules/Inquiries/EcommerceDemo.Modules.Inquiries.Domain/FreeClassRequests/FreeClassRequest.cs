using System.Text.RegularExpressions;

namespace EcommerceDemo.Modules.Inquiries.Domain.FreeClassRequests;

public sealed partial class FreeClassRequest
{
    public const int OwnerNameMaxLength = 120;
    public const int PhoneMaxLength = 32;
    public const int BreedMaxLength = 80;
    public const int DogAgeMaxLength = 40;
    public const int BehaviorProblemMaxLength = 2000;
    public const int PhoneMinDigits = 7;
    public const int PhoneMaxDigits = 15;

    private FreeClassRequest()
    {
    }

    public Guid Id { get; private set; }

    public string OwnerName { get; private set; } = string.Empty;

    public string Phone { get; private set; } = string.Empty;

    public string Breed { get; private set; } = string.Empty;

    public string DogAge { get; private set; } = string.Empty;

    public string BehaviorProblem { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public static FreeClassRequest Create(
        Guid id,
        string ownerName,
        string phone,
        string breed,
        string dogAge,
        string behaviorProblem,
        DateTimeOffset createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id is required.", nameof(id));
        }

        var normalizedOwner = RequireText(ownerName, OwnerNameMaxLength, nameof(ownerName));
        var normalizedPhone = RequirePhone(phone);
        var normalizedBreed = RequireText(breed, BreedMaxLength, nameof(breed));
        var normalizedAge = RequireText(dogAge, DogAgeMaxLength, nameof(dogAge));
        var normalizedProblem = RequireText(behaviorProblem, BehaviorProblemMaxLength, nameof(behaviorProblem));

        if (createdAt == default)
        {
            throw new ArgumentException("CreatedAt is required.", nameof(createdAt));
        }

        return new FreeClassRequest
        {
            Id = id,
            OwnerName = normalizedOwner,
            Phone = normalizedPhone,
            Breed = normalizedBreed,
            DogAge = normalizedAge,
            BehaviorProblem = normalizedProblem,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }

    public static bool IsPlausiblePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return false;
        }

        var trimmed = phone.Trim();
        if (trimmed.Length > PhoneMaxLength || !AllowedPhone().IsMatch(trimmed))
        {
            return false;
        }

        var digits = 0;
        foreach (var character in trimmed)
        {
            if (char.IsAsciiDigit(character))
            {
                digits++;
            }
        }

        return digits is >= PhoneMinDigits and <= PhoneMaxDigits;
    }

    private static string RequireText(string value, int maxLength, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > maxLength)
        {
            throw new ArgumentException($"Value is required and must be at most {maxLength} characters.", paramName);
        }

        return value.Trim();
    }

    private static string RequirePhone(string phone)
    {
        if (!IsPlausiblePhone(phone))
        {
            throw new ArgumentException("Phone must contain 7 to 15 digits and no letters.", nameof(phone));
        }

        return phone.Trim();
    }

    [GeneratedRegex(@"^\+?[0-9][0-9\s().-]{6,31}$")]
    private static partial Regex AllowedPhone();
}
