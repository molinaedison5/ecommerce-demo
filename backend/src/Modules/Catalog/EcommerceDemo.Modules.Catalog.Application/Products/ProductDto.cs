namespace EcommerceDemo.Modules.Catalog.Application.Products;

public sealed record ProductDto(Guid Id, string Name, string Description, decimal Price, string Currency, string Sku);
