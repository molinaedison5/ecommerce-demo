using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EcommerceDemo.Modules.Inquiries.Infrastructure.Persistence;

public sealed class InquiriesDbContextFactory : IDesignTimeDbContextFactory<InquiriesDbContext>
{
    public InquiriesDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<InquiriesDbContext>()
            .UseNpgsql(
                "Host=localhost;Port=5432;Database=ecommerce;Username=ecommerce;Password=ecommerce",
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", InquiriesDbContext.Schema))
            .Options;

        return new InquiriesDbContext(options);
    }
}
