using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EcommerceDemo.Modules.Catalog.Infrastructure.Persistence;

public sealed class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseNpgsql(
                "Host=localhost;Port=5432;Database=ecommerce;Username=ecommerce;Password=ecommerce",
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", CatalogDbContext.Schema))
            .Options;

        return new CatalogDbContext(options);
    }
}
