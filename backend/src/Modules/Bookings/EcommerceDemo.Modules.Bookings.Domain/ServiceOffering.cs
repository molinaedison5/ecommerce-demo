namespace EcommerceDemo.Modules.Bookings.Domain;

public enum CareServiceKind
{
    Grooming = 1,
    VetVisit = 2,
    Daycare = 3,
}

public sealed class ServiceOffering
{
    private ServiceOffering()
    {
    }

    public Guid Id { get; private set; }

    public CareServiceKind Kind { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int DurationMinutes { get; private set; }

    public static ServiceOffering Create(Guid id, CareServiceKind kind, string name, int durationMinutes)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (durationMinutes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(durationMinutes), "Duration must be positive.");
        }

        return new ServiceOffering
        {
            Id = id,
            Kind = kind,
            Name = name.Trim(),
            DurationMinutes = durationMinutes,
        };
    }
}
