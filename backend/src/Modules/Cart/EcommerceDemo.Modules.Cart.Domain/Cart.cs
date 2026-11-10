namespace EcommerceDemo.Modules.Cart.Domain;

public sealed class CartLine
{
    public CartLine(Guid productId, int quantity)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product is required.", nameof(productId));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
        }

        ProductId = productId;
        Quantity = quantity;
    }

    public Guid ProductId { get; }

    public int Quantity { get; }
}

public sealed class Cart
{
    private readonly List<CartLine> _lines = [];

    public Cart(Guid id, Guid customerId)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id is required.", nameof(id));
        }

        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("Customer is required.", nameof(customerId));
        }

        Id = id;
        CustomerId = customerId;
    }

    public Guid Id { get; }

    public Guid CustomerId { get; }

    public IReadOnlyList<CartLine> Lines => _lines;

    public void Add(CartLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        _lines.Add(line);
    }
}
