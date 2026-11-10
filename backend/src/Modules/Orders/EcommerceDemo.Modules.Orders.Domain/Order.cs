namespace EcommerceDemo.Modules.Orders.Domain;

public enum OrderStatus
{
    Draft = 1,
    Placed = 2,
    Paid = 3,
    Cancelled = 4,
}

public sealed class Order
{
    private Order()
    {
    }

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public OrderStatus Status { get; private set; }

    public static Order Draft(Guid id, Guid customerId)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id is required.", nameof(id));
        }

        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("Customer is required.", nameof(customerId));
        }

        return new Order
        {
            Id = id,
            CustomerId = customerId,
            Status = OrderStatus.Draft,
        };
    }
}
