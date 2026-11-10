using EcommerceDemo.Modules.Catalog.Domain.Products;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceDemo.Modules.Catalog.Infrastructure.Persistence;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(product => product.Id);
        builder.Property(product => product.Id).HasColumnName("id");
        builder.Property(product => product.Sku).HasColumnName("sku").HasMaxLength(64).IsRequired();
        builder.Property(product => product.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(product => product.Description).HasColumnName("description").HasMaxLength(2000).IsRequired();
        builder.Property(product => product.PriceAmount).HasColumnName("price_amount").HasPrecision(12, 2);
        builder.Property(product => product.PriceCurrency).HasColumnName("price_currency").HasMaxLength(3).IsFixedLength().IsRequired();
        builder.Property(product => product.IsActive).HasColumnName("is_active");
        builder.HasIndex(product => product.Sku).IsUnique();
    }
}
