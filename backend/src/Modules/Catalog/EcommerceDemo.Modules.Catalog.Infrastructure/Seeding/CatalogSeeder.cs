using EcommerceDemo.Modules.Catalog.Domain.Products;
using EcommerceDemo.Modules.Catalog.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace EcommerceDemo.Modules.Catalog.Infrastructure.Seeding;

internal static class CatalogSeeder
{
    public static async Task SeedAsync(CatalogDbContext db, CancellationToken cancellationToken)
    {
        if (await db.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        db.Products.AddRange(
            Product.Create(
                Guid.Parse("6f1c1a10-1f4a-4c0e-9b3a-111111111111"),
                "DOG-FOOD-001",
                "Alimento premium para perros adultos",
                "Croquetas con pollo para perros adultos. Bolsa de demostración de 2 kg.",
                24.90m,
                "USD"),
            Product.Create(
                Guid.Parse("6f1c1a10-1f4a-4c0e-9b3a-222222222222"),
                "CAT-LITTER-001",
                "Arena aglomerante para gatos",
                "Arena de bentonita, bajo polvo, pensada para areneros cubiertos.",
                12.50m,
                "USD"),
            Product.Create(
                Guid.Parse("6f1c1a10-1f4a-4c0e-9b3a-333333333333"),
                "FLEA-COLLAR-001",
                "Collar antipulgas ajustable",
                "Collar de demostración para perros medianos. No es un producto médico real.",
                15.00m,
                "USD"),
            Product.Create(
                Guid.Parse("6f1c1a10-1f4a-4c0e-9b3a-444444444444"),
                "GROOM-BRUSH-001",
                "Cepillo de doble cara",
                "Cepillo para pelaje corto y medio. Útil antes de una sesión de peluquería.",
                9.99m,
                "USD"));

        await db.SaveChangesAsync(cancellationToken);
    }
}
