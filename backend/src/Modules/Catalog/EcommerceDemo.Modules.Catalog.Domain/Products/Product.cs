using EcommerceDemo.BuildingBlocks.Domain;

namespace EcommerceDemo.Modules.Catalog.Domain.Products;

public sealed class Product
{
    private Product()
    {
    }

    public Guid Id { get; private set; }

    public string Sku { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public decimal PriceAmount { get; private set; }

    public string PriceCurrency { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public static Product Create(Guid id, string sku, string name, string description, decimal priceAmount, string currency)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(sku) || sku.Trim().Length > 64)
        {
            throw new ArgumentException("SKU is required and must be at most 64 characters.", nameof(sku));
        }

        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 200)
        {
            throw new ArgumentException("Name is required and must be at most 200 characters.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(description) || description.Trim().Length > 2000)
        {
            throw new ArgumentException("Description is required and must be at most 2000 characters.", nameof(description));
        }

        var price = new Money(priceAmount, currency);
        return new Product
        {
            Id = id,
            Sku = sku.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            Description = description.Trim(),
            PriceAmount = price.Amount,
            PriceCurrency = price.Currency,
            IsActive = true,
        };
    }
}
